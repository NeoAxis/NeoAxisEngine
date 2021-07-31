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


#ifndef W_REQUEST_H
#define W_REQUEST_H


#include <ptypes.h>
#include <pstreams.h>
#include <pinet.h>

#include "sysutils.h"
#include "urlutils.h"


USING_PTYPES


enum http_version_t {
    HTTP_VER_09, 
    HTTP_VER_10, 
    HTTP_VER_11, 
    HTTP_VER_UNKNOWN,
        HTTP_VER_MAX
};


enum http_method_t {
    HTTP_GET,
    HTTP_HEAD,
    HTTP_OTHER,
        HTTP_METHOD_MAX
};


enum req_stat_t
{
    STAT_READ,
    STAT_WRITE,
    STAT_WAIT,
        STAT_MAX
};


struct ehttp
{
    int code;
    ehttp(int icode): code(icode) {}
};


class request_rec
{
public:
    datetime        started;
    int             rsp_code;       // rsp: response code, set through begin_response(), used for logging
    req_stat_t      stat;           // for status requests: READ, WRITE or WAIT (if keep-alive)
    instm*          sockin;         // client input stream
    outstm*         sockout;        // client output stream
    ipaddress       client_ip;      // req: client's IP
    http_version_t  version;        // req/rsp: HTTP version, 0.9, 1.0, 1.1 or 1.*
    http_method_t   method;         // req: HTTP method, currently GET or HEAD
    string          method_str;     // method string, for other method handlers
    bool            keep_alive;     // req/rsp: whether to close the connection; determined
                                    // based on the HTTP version and the "Connection:" header;
                                    // can be forced to false for some response types, e.g. 400 Bad Request
    datetime        if_modified;    // req: "If-modified-since:" if present, invdatetime otherwise
    string          req_line;       // req: the whole request line, e.g. "GET / HTTP/1.1"
    string          uri;            // req: request-URI, as is
    string          host;           // req: "Host:" header, if present
    string          referer;        // req: "Referer:"; may be converted to relative URI
    bool            partial;        // req: partial content requested (see rsp_file())
    large           range_min;      // req: partial content
    large           range_max;      // req: partial content
    textmap         headers;        // other headers

    // requested object info
    urlrec          url;            // req: the request-URI parsed and split into components
    strlist         path_parts;     // request-URI path split into components

    unknown*        user;           // user data for custom handlers, freed automatically by ~request_rec()

    // helpers for method handlers
    string get_token(const cset& chars);
    string get_uri();
    void parse_request_line();      // ... excluding the method string
    void parse_hdr(string& fname, string& fvalue);
    void parse_uri();               // sets url and path_parts fields
    void analyze_uri();             // sets all fields starting from file_type

    // response utilities; headers are not sent if the HTTP version is 0.9
    void begin_response(int code, const char* msg);

    void put_header(const char* name, const char* value);
    void put_header(const char* name, const string& value);
    void put_content_type(const char* mime);
    void put_content_length(large length);
    void end_headers();
    void std_response(bool conn_close, int code, const char* msg, const char* descr);
    void std_response(bool conn_close, int code, const char* msg, const char* descr, const string& dparam);

    // standard responses; all functions of this group raise ehttp exceptions
    void rsp_not_found();
    void rsp_bad_request();
    void rsp_bad_method(const char* ok_methods);
    void rsp_uri_too_long();
    void rsp_forbidden();
    void rsp_dir_index_forbidden();
    void rsp_overloaded();
    void abort_request();
    void rsp_redirect(const string& newurl);
    void rsp_not_modified();

    void end_response();

protected:
    string location;       // rsp: add "Location:" header to the response; set through 
                           // rsp_redirect()
    int    hdr_size;       // byte size of response headers; used for logging, to determine
                           // the actual response content length (see respond())

    // request parsers
    void parse_method();
    void parse_headers();

    // the boss
    void respond();

    // reset the state between requests when keep-alive
    void reset_state();

    request_rec(instm& isockin, outstm& isockout, ipaddress iclient_ip);
    ~request_rec();
};


// this structure is created and handled in mod_file, however, we
// declare it here since this information can be passed to the
// file extension handlers.

class file_request_rec
{
public:
    file_type_t     file_type;      // file, directory or other (device or pipe)
    bool            sym_link;       // the object is a symbolic link (Unix only)
    bool            executable;     // the object is executable (binary on Unix, .exe on Windows)
    string          abs_path;       // absolute file path to the requested object
    string          rel_path;       // file path to the requested object relative to document root,
                                    // may not be the same as url.path
    string          file_name;      // file name
    string          file_ext;       // file extension, including the leading dot

    file_request_rec(): file_type(FT_ERROR), sym_link(false), executable(false),
        abs_path(), rel_path(), file_name(), file_ext()  {}
    ~file_request_rec()  {}
};


#endif
