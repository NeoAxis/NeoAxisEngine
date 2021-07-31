/*
 *
 *  C++ Portable Types Library (PTypes)
 *  Version 2.1.1  Released 27-Jun-2007
 *
 *  Copyright (C) 2001-2007 Hovik Melikyan
 *
 *  http://www.melikyan.com/ptypes/
 *
 */

//
// file handling module; registered as a path handler for '/'
//

#include <limits.h>

#include "config.h"
#include "modules.h"
#include "request.h"
#include "utils.h"


class file_req: public file_request_rec
{
protected:
    request_rec* req;

    void analyze_uri();
    void rsp_dir_index();
    void rsp_file();

public:
    file_req(request_rec* req);
    ~file_req();

    void respond();
};


file_req::file_req(request_rec* ireq): file_request_rec(), req(ireq)
{
}


file_req::~file_req()
{
}


void file_req::analyze_uri()
{
    abs_path = cfg_document_root + '/';
    rel_path = '/';
    file_type = FT_DIRECTORY;

    // analyze path components one by one and stop where the
    // component does not exist, or where we see a file, not a
    // directory. note, that the request-uri may be longer and
    // may contain extra components which are ignored. f.ex.
    // the request uri is /dir/file.html/other/stuff/. the
    // scan stops at file.html and ignores the rest. this is the
    // way apache and most other daemons work.
    // besides, we may pass a non-existent path to a registered
    // path handler.

    for (int p = 0; p < req->path_parts.get_count(); ++p)
    {
        string s = req->path_parts.getkey(p);
        abs_path += s;
        rel_path += s;

        file_type = get_file_type(abs_path);
        if (file_type == FT_DIRECTORY)
        {
            abs_path += '/';
            rel_path += '/';
        }
        else if (file_type == FT_FILE)
        {
            file_name = s;
            break;
        }
        else if (file_type == FT_OTHER) // pipe or device
            req->rsp_forbidden();
        else
            req->rsp_not_found();
    }

    if (file_type == FT_DIRECTORY)
    {
        // check for the trailing slash and redirect to the canonical
        // form if necessary
        if (trail_char(req->url.path) != '/')
        {
            req->url.path += '/';
            req->rsp_redirect(urlcreate(req->url));
        }

        // find the default index file
        if (cfg_def_index_files)
        {
            const char** idx = cfg_index_files;
            while (*idx != nil)
            {
                string t = abs_path + *idx;
                if (is_file(t))
                {
                    abs_path = t;
                    file_type = FT_FILE;
                    file_name = *idx;
                    break;
                }
                idx++;
            }
        }
    }

    // other useful info about the object
    sym_link = is_symlink(abs_path);
    executable = is_executable(abs_path);
    if (!isempty(file_name))
    {
        file_ext = get_file_ext(file_name);
        handler_info* h = find_file_handler(file_ext);
        if (h != 0)
        {
            file_callback(h->callback)(*req, *this);
            // the handler must throw an ehttp exception
            fatal(251, "Internal error 251");
        }
    }
}


void file_req::rsp_dir_index()
{
    if (!cfg_dir_indexes)
        req->rsp_dir_index_forbidden();

    filist list(SL_SORTED | SL_CASESENS | SL_OWNOBJECTS);
    get_directory(list, abs_path, true, 500);
    if (list.get_count() == 0)
        req->rsp_forbidden();

    // we don't know the length of the resulting file
    req->keep_alive = false;

    req->begin_response(200, "OK");
    req->put_content_type("text/html");
    req->end_headers();

    // build an index HTML page
    std_html_header(*req->sockout, "Index of " + rel_path);
    req->sockout->put("<hr noshade size=1>\n");
    req->sockout->put("<pre>\n");
    html_show_file_list(*req->sockout, list);
    list.clear();
    get_directory(list, abs_path, false, 500);
    html_show_file_list(*req->sockout, list);

    req->sockout->put("</pre>\n");
    std_html_footer(*req->sockout);

    req->end_response();
}


void file_req::rsp_file()
{
    // .ht* files are forbidden, like with Apache
    if (strncmp(file_name, ".ht", 3) == 0)
        req->rsp_forbidden();

    large fsize64 = get_file_size(abs_path);
    if (fsize64 < 0)
        req->rsp_not_found();

    // test the file for readability
    infile f(abs_path);
    try
    {
        f.set_bufsize(0);
        f.open();
    }
    catch(estream* e)
    {
        delete e;
        req->rsp_forbidden();
    }

    // partial content
    large txsize = fsize64;
    if (req->partial)
    {
        if (req->range_max == -1)
            req->range_max = fsize64 - 1;
        if (req->range_min >= 0 && req->range_min < req->range_max && req->range_max < fsize64)
            txsize = req->range_max - req->range_min + 1;
        else
            req->partial = false;
    }

    // send headers
    datetime fmtime = get_file_mtime(abs_path);
    if (req->if_modified != invdatetime && fmtime != invdatetime && fmtime <= req->if_modified)
        req->rsp_not_modified();

    if (req->partial)
        req->begin_response(206, "Partial Content");
    else
        req->begin_response(200, "OK");
    req->put_content_type(get_mimetype(abs_path));
    req->put_content_length(txsize);
    if (req->partial)
        req->put_header("Content-Range",
            "bytes " + itostring(req->range_min) + '-' + itostring(req->range_max) + '/' + itostring(fsize64));
    if (cfg_file_mtime)
        req->put_header("Last-Modified", http_time_stamp(fmtime));
    req->end_headers();

    // send content
    if (req->partial)
        f.seekx(req->range_min);
    char buf[FILE_BUF_SIZE];
    while (txsize > 0)
    {
        int toread = sizeof(buf);
        if (toread > txsize)
            toread = (int)txsize;
        int r = f.read(buf, toread);
        if (r <= 0)
            break;
        req->sockout->write(buf, r);
        txsize -= r;
    }

    // if for some reason the number of bytes sent does not correspond to the
    // promised content length, just close the connection
    if (txsize != 0)
        req->keep_alive = false;

    req->end_response();
}


void file_req::respond()
{
    if (req->method != HTTP_GET && req->method != HTTP_HEAD)
        req->rsp_bad_method("GET, HEAD");

    analyze_uri();

    if (file_type == FT_DIRECTORY)
        rsp_dir_index();

    else if (file_type == FT_FILE)
        rsp_file();

    else
        req->rsp_not_found();
}


void handle_file(request_rec& req)
{
    file_req freq(&req);
    freq.respond();
}
