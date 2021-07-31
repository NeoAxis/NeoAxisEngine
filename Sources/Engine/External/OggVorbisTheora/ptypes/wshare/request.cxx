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

#include <stdlib.h>
#include <stdio.h>
#include <limits.h>

#include "config.h"
#include "log.h"
#include "utils.h"
#include "sysutils.h"
#include "request.h"
#include "clients.h"
#include "modules.h"


USING_PTYPES


const char* http_version_str[HTTP_VER_MAX] = {"", "HTTP/1.0", "HTTP/1.1", "HTTP/1.1"};
// const char* http_method_str[HTTP_METHOD_MAX] = {"GET", "HEAD", ""};


request_rec::request_rec(instm& isockin, outstm& isockout, ipaddress iclient_ip)
    : started(now()), rsp_code(0), stat(STAT_READ), sockin(&isockin), sockout(&isockout), client_ip(iclient_ip), 
      version(HTTP_VER_10), method(HTTP_GET), method_str(), keep_alive(false), if_modified(invdatetime), 
      req_line(), uri(), host(), referer(), partial(false), range_min(0), range_max(0), headers(),
      url(), path_parts(), user(0),
      location(), hdr_size(0)
{
}


void request_rec::reset_state()
{
    rsp_code = 0;
    version = HTTP_VER_10;
    method = HTTP_GET;
    clear(method_str);
    keep_alive = false;
    if_modified = invdatetime; 
    clear(req_line);
    clear(uri);
    path_parts.clear();
    clear(host);
    clear(referer);
    partial = false;
    range_min = 0;
    range_max = 0;
    headers.clear();
    urlclear(url);
    delete user;
    user = 0;
    clear(location);
    hdr_size = 0;
}


request_rec::~request_rec()
{
    delete user;
}


void request_rec::put_header(const char* name, const char* value)
{
    if (version > HTTP_VER_09)
    {
        sockout->put(name);
        sockout->put(": ");
        sockout->put(value);
        sockout->put("\r\n");
    }
}


void request_rec::put_header(const char* name, const string& value)
{
    if (version > HTTP_VER_09)
    {
        sockout->put(name);
        sockout->put(": ");
        sockout->put(value);
        sockout->put("\r\n");
    }
}


void request_rec::put_content_type(const char* mime)
{
    put_header("Content-Type", mime);
}


void request_rec::put_content_length(large length)
{
    if (method != HTTP_HEAD)
        put_header("Content-Length", itostring(length));
}


void request_rec::end_headers()
{
    if (version > HTTP_VER_09)
        sockout->put("\r\n");
    hdr_size = sockout->tell();
    if (method == HTTP_HEAD)
        end_response();
}


void request_rec::begin_response(int code, const char* msg)
{
    rsp_code = code;
    stat = STAT_WRITE;

    if (version > HTTP_VER_09)
    {
        sockout->putf("%s %d %s\r\n", http_version_str[version], code, msg);
        put_header("Date", http_time_stamp(now(true)));
        put_header("Server", SERVER_APP_NAME);
        //    put_header("Accept-Ranges", "bytes");
        
        if (!isempty(location))
            put_header("Location", location);
        
        static const char* sconn[2] = {"close", "keep-alive"};
        if (version < HTTP_VER_11)
            put_header("Connection", sconn[keep_alive]);
        else if (!keep_alive)   // HTTP/1.1
            put_header("Connection", "close");
    }
}


void request_rec::std_response(bool conn_close, int code, const char* msg, const char* descr)
{
    if (conn_close)
        keep_alive = false;

    // we need a memory stream to temporarily store the response
    outmemory s(4096);
    string smsg = msg;

    // write out the standard response page in HTML format
    // to the memory stream
    s.open();
    if (strlen(descr) != 0)
    {
        std_html_header(s, itostring(code) + ' ' + smsg);
        s.put("<p>");
        html_encode(s, descr);
        s.put("</p>\n");
        std_html_footer(s);
    }

    // send the response
    begin_response(code, msg);
    if (s.tell() > 0)    // some responses do not return any content, e.g. 304
    {
        put_content_type("text/html");
        put_content_length(s.tell());
    }
    end_headers();
    if (s.tell() > 0)
        sockout->put(s.get_strdata());

    end_response();
}


void request_rec::std_response(bool conn_close, int code, const char* msg, const char* descr, const string& dparam)
{
    char buf[1024];
    snprintf(buf, sizeof(buf), descr, pconst(dparam));
    std_response(conn_close, code, msg, buf);
}


void request_rec::rsp_not_found()
{
    std_response(false, 404, "Not found", "The requested object %s was not found on this server.", url.path);
}


void request_rec::rsp_bad_request()
{
    std_response(true, 400, "Bad request", "Your browser sent a request that this server could not understand.");
}


void request_rec::rsp_bad_method(const char* ok_methods)
{
    put_header("Allow", ok_methods);
    std_response(true, 405, "Method not allowed", "Method %s not allowed for this resource", method_str);
}


void request_rec::rsp_uri_too_long()
{
    std_response(true, 414, "Request-URI too long", "The request-URI string sent by your browser is too long.");
}


void request_rec::rsp_forbidden()
{
    std_response(false, 403, "Forbidden", "You don't have permission to access %s on this server", url.path);
}


void request_rec::rsp_dir_index_forbidden()
{
    std_response(false, 403, "Directory index forbidden", "Directory index forbidden: %s", url.path);
}


void request_rec::rsp_redirect(const string& newurl)
{
    location = newurl;
    std_response(false, 301, "Moved permanently", "The document has moved to %s", newurl);
}


void request_rec::rsp_overloaded()
{
    std_response(true, 503, "Service unavailable", "The server is overloaded. Please, try again later.");
}


void request_rec::rsp_not_modified()
{
    std_response(false, 304, "Not modified", "");
}


void request_rec::abort_request()
{
#ifdef DEBUG
    syslog_write(SYSLOG_WARNING, "Request from %s aborted", pconst(iptostring(client_ip)));
#endif
    keep_alive = false;
    throw ehttp(0);
}


void request_rec::end_response()
{
    throw ehttp(rsp_code);
}


//
// request parsers
//

const cset method_chars = "A-Z";
const cset uri_chars = "~21-~FF";
const cset field_chars = uri_chars - cset(":");
const cset ws_chars = "~20";


string request_rec::get_token(const cset& chars)
{
    char buf[MAX_TOKEN];
    int bufsize = sockin->token(chars, buf, sizeof(buf));
    if (bufsize == 0 || bufsize >= MAX_TOKEN)
        rsp_bad_request();
    return string(buf, bufsize);
}


string request_rec::get_uri()
{
    char buf[MAX_REQUEST_URI];
    int bufsize = sockin->token(uri_chars, buf, sizeof(buf));
    if (bufsize == 0)
        rsp_bad_request();
    if (bufsize >= MAX_REQUEST_URI)
        rsp_uri_too_long();
    return string(buf, bufsize);
}


void request_rec::parse_method()
{
    while (!sockin->get_eof() && sockin->get_eol())
        sockin->skipline();
    method_str = get_token(method_chars);
    req_line = method_str;

    // try to pass this method to a registered method handler.
    // the rest of the request line can be parsed using 
    // parse_request_line(), if it's HTTP/1.1-like.
    handler_info* h = find_method_handler(method_str);
    if (h != 0)
    {
        method_callback(h->callback)(*this);
        // the handler must throw an ehttp exception
        fatal(252, "Internal error 252");
    }
    // otherwise use the internal method handlers
    else if (method_str == "GET")
        method = HTTP_GET;
    else if (method_str == "HEAD")
        method = HTTP_HEAD;
    else if (length(method_str) == 0)
        abort_request();
    else
        rsp_bad_method("GET, HEAD");
}


void request_rec::parse_request_line()
{
    if (sockin->skiptoken(ws_chars) == 0)
        abort_request();

    // read the request URI
    uri = get_uri();
    req_line += ' ' + uri;

    // read the version number, if present
    if (sockin->get_eol())
        version = HTTP_VER_09;
    else
    {
        string s;

        if (sockin->skiptoken(ws_chars) == 0)
            abort_request();
        s = get_token(uri_chars);
        req_line += ' ' + s;
        const char* p = s;
        if (length(s) < 8 || strncmp(p, "HTTP/1.", 7) != 0)
            rsp_bad_request();
        if (p[7] == '0')
            version = HTTP_VER_10;
        else if (p[7] == '1')
            version = HTTP_VER_11;
        else
            version = HTTP_VER_UNKNOWN; // 1.x is ok for us
    }

    // HTTP/1.1 requires to keep the connection alive by default;
    // can be overridden by `Connection:' header
    keep_alive = version >= HTTP_VER_11;

    if (!sockin->get_eol())
        rsp_bad_request();

    if (version > HTTP_VER_09)
        sockin->skipline();
}


void request_rec::parse_hdr(string& fname, string& fvalue)
{
    fname = get_token(field_chars);             // read the field name
    sockin->skiptoken(ws_chars);
    if (sockin->get() != ':')                   // malformed header (no colon)
        rsp_bad_request();
    
    do {
        sockin->skiptoken(ws_chars);            // skip leading ws chars
        do {
            if (sockin->get_eol())              // the value may be empty (?)
		break;
            string t = get_token(uri_chars);    // read field value
            if (!isempty(fvalue))
                fvalue += ' ';
            fvalue += t;
            if (length(fvalue) > MAX_TOKEN)
                rsp_bad_request();
        // according to RFC2616 all ws chars inside the field value
        // can become a single space
        } while (sockin->skiptoken(ws_chars) > 0);
        
        if (!sockin->get_eol())
            rsp_bad_request();
        sockin->skipline();
    } while (sockin->preview() & ws_chars); // see if field value continues on the next line
}


void request_rec::parse_headers()
{
    while (!sockin->get_eol())
    {
        string fname, fvalue;
        parse_hdr(fname, fvalue);
        fname = lowercase(fname);

        if (fname == "host")
            host = fvalue;

        else if (fname == "connection")
        {
            fvalue = lowercase(fvalue);
            if (fvalue == "close")
                keep_alive = false;
            else if (fvalue == "keep-alive")
                keep_alive = true;
        }

        else if (fname == "if-modified-since")
        {
            if_modified = parse_http_date(fvalue);
            if (if_modified == invdatetime)
                rsp_bad_request();
        }

        else if (fname == "referer")
            referer = fvalue;

        else if (fname == "range")
        {
            if (strncmp(fvalue, "bytes=", 6) == 0)
            {
                del(fvalue, 0, 6);
                const char* p = fvalue;
                char* e;
                int rmin = strtol(p, &e, 10);
                if (*e == '-')
                {
                    p = e + 1;
                    int rmax = strtol(p, &e, 10);
                    if (e == p)
                        rmax = -1;
                    // we don't support multiple ranges, neither negative ranges
                    if (*e == 0 && rmin >= 0)
                    {
                        partial = true;
                        range_min = rmin;
                        range_max = rmax;
                    }
                }
            }
        }

        // other headers go to request_rec::headers for use in 
        // custom plugins/modules. so called "coalesce" headers
        // are not supported yet
        else
            headers.put(fname, fvalue);
    }

    // convert the referer URI to relative if on the same host
    if (!isempty(referer))
    {
        string s = "http://" + host;
        if (strncmp(s, referer, length(s)) == 0)
            del(referer, 0, length(s));
        if (isempty(referer))
            referer = "/";
    }

    sockin->skipline();
}


void request_rec::parse_uri()
{
    string s = uri;
    if (!isurl(uri))
    {
        // if request URI is just a path
        if (*pconst(uri) != '/')
            rsp_bad_request();
        if (version > HTTP_VER_09 && isempty(host))
            rsp_bad_request();
        s = "http://" + host + uri;
    }

    urlcrack(s, url);

    // split the path into components
    split_path(url.path, path_parts);
}


void request_rec::analyze_uri()
{
    // analyze path components one by one and stop at the longest
    // match that has a path handler. note that the request-uri
    // may be longer and may contain extra components which are
    // ignored, but left intact to be used by the handler.

    string path = "/";
    handler_info* handler = find_path_handler(path);

    for (int i = 0; i < path_parts.get_count(); i++)
    {
        path += path_parts.getkey(i);
        handler_info* h = find_path_handler(path);
        if (h != 0)
            handler = h;
        path += "/";
    }

    if (handler == 0)
        rsp_not_found();

    path_callback(handler->callback)(*this);
    // the handler must throw an ehttp exception
    fatal(253, "Internal error 253");
}


void request_rec::respond()
{
    try
    {
        if (thread_count > cfg_max_clients)
            rsp_overloaded();

        parse_method();
        parse_request_line();
        parse_headers();
        parse_uri();
        analyze_uri();

        // all branches must throw an ehttp exception
        // before reaching this point
        fatal(254, "Internal error 254");
    }

    catch(ehttp e)
    {
        if (keep_alive)
        {
            sockout->flush();
            stat = STAT_WAIT;
        }
        else
        {
            sockin->close();
            sockout->close();
        }

        htlog_write(client_ip, req_line, e.code, sockout->tellx() - hdr_size, referer);
    }
}


