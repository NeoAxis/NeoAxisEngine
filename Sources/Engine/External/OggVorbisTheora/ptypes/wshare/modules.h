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

#ifndef W_MODULES_H
#define W_MODULES_H


#include <ptypes.h>

#include "request.h"


USING_PTYPES


//
// wshare modules API
//

// we currently only support statically linked handlers. all handlers
// receive request_rec& as a parameter and must respond by either
// calling one of the request_rec::rsp_XXX standard functions, or by
// performing some actions and giving the ehttp exception with the HTTP
// response code as a parameter. all public methods and fields of 
// request_rec are at your disposal. all rsp_XXX methods throw exceptions
// of type ehttp, so they never return to the caller.

// the following macros must be placed in init_handlers() (modules.cxx)
// for each new handler. the handler callback must exist somewhere and
// must be linked to wshare. these macros declare your callback functions
// as extern's, so you don't need to do it elsewhere.

// there are three types of handlers:

//   method handlers are called whenever an unknown HTTP method is found
//   in the request line. the rest of input data must be processed by this
//   handler. you can call request_rec::parse_XXX if the request is 
//   HTTP/1.1-like. default methods can be overridden.

#define ADD_METHOD_HANDLER(method,callback)   \
    extern void callback(request_rec&);       \
    add_method_handler(method, callback);


//   path handlers are called when the request-URI matches the path for a
//   registered handler. the request-URI path is checked one path_part at
//   a time against the paths given here. the longest match wins:
//   ex.: consider the uri path "/stuff/things/widget" matched against
//   this handler list:
//      "/"                     --> matches, but not the longest
//      "/stuff"                --> matches, but not the longest
//      "/stuff/things"         --> matches, the longest match
//      "/stuff/things/gizmo    --> doesn't match
//      "/stuff/thin"           --> doesn't match (partial parts are not matched)
//   all request_rec fields up to `parts_used' inclusive contain valid values
//   which can be used by this handler.  It is also up to the handler to check
//   that it can deal with the requested method (GET/HEAD/POST)
//   the path parameter must contain the leading slash, and must not
//   contain the trailing slash.

#define ADD_PATH_HANDLER(path,callback)       \
    extern void callback(request_rec&);       \
    add_path_handler(path, callback);


//   file handlers are called for specific file extensions for
//   existing files. the ext paramter is the extension this handler
//   wishes to handle. the ext parameter must contain the leading dot.

#define ADD_FILE_HANDLER(ext,callback)        \
    extern void callback(request_rec&);       \
    add_file_handler(ext, callback);


// all handler functions must be of the following types (depending on the
// handler type):

typedef void (*method_callback)(request_rec& req);
typedef void (*path_callback)(request_rec& req);
typedef void (*file_callback)(request_rec& req, file_request_rec& freq);


//
// internal module management
//


struct handler_info
{
    handler_info* next;
    void* callback;
    string param;

    handler_info(handler_info*, void*, const string&);
};


handler_info* find_method_handler(const string& method);
handler_info* find_path_handler(const string& path);
handler_info* find_file_handler(const string& ext);


void init_handlers();


void add_method_handler(const string& method, method_callback);
void add_path_handler(const string& path, path_callback);
void add_file_handler(const string& ext, file_callback);


#endif
