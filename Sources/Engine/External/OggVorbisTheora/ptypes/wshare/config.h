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

#ifndef W_CONFIG_H
#define W_CONFIG_H


#include <ptypes.h>


USING_PTYPES


// things that can hardly change

#define MAX_TOKEN               4096
#define MAX_REQUEST_URI         4096
#define FILE_BUF_SIZE		4096
#define SOCKET_BUF_SIZE         2048
#define DEF_HTTP_PORT           80
#define DEF_KEEP_ALIVE_TIMEOUT  15000
#define DEF_MAX_CLIENTS         30


// we no longer use strftime for HTTP time stamps, since it may
// depend on the system locale. now we build the time stamp 
// `manually' instead
// #define HTTP_DATE_FMT           "%a, %d %b %Y %H:%M:%S GMT"


// built-in configuration

extern const char*   SERVER_APP_NAME;   // "wshare/1.2 (system)"
extern const char*   DEF_MIME_TYPE;     // "text/plain" for files with no extension
// not implemented: extern const char*   DEF_BINARY_TYPE;   // "application/octet-stream" for unix binaries
extern const char*   STD_CSS;           // CSS for built-in responses, such like 404 Not found

extern const char*   cfg_index_files[]; // index.html, Index.html and default.htm
extern const char*   mimetypes[];       // built-in MIME types and extensions (in mimetable.cxx)


// configurable parameters

extern string  cfg_document_root;       // no default, must be specified through cmdline
extern string  cfg_server_name;         // default is my nodename
extern int     cfg_port;                // 80
extern bool    cfg_dir_indexes;         // allow directory indexes
extern bool    cfg_def_index_files;     // use default index files index.html, ...
extern string  cfg_log_file;            // access log file name
extern bool    cfg_log_referer;         // include referer in log
extern bool    cfg_file_mtime;          // send `last-modified' for html files
extern int     cfg_max_clients;         // max number of simultaneous connections, default 30
extern int     cfg_max_keep_alive;      // max keep-alive clients, currently 2/3 of max_clients

extern bool    cfg_syslog;              // write the log to stderr or to Unix syslog
extern string  cfg_user;                // user name to run as on Unix
extern string  cfg_group;               // group name to run as on Unix
extern bool    cfg_daemonize;           // daemonize on Unix

extern char*   myname;                  // "wshare", for syslog


void config_init(int argc, char* argv[]);
void config_done();


#ifdef _MSC_VER
// we don't want "conditional expression is constant" warning
#  pragma warning (disable: 4127)
#endif


#endif
