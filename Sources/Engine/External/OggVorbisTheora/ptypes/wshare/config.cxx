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

#include "ptypes.h"

#include "config.h"


USING_PTYPES


const char* SERVER_APP_NAME = "wshare/1.3"
#ifdef WIN32
    " (Win32)";
#else
    " (UNIX)";
#endif

const char* DEF_MIME_TYPE = "text/plain";                 // no extension
const char* DEF_BINARY_TYPE = "application/octet-stream"; // for unix binaries with no extension


const char*  STD_CSS = 
    "<style type=text/css>\n"
    "<!--\n"
    "body{font-family:Verdana,sans-serif;font-size:9pt}\n"
    "h3{font-size:13pt}\n"
    "pre{font-family:Courier New,cour}\n"
    "-->\n"
    "</style>\n";


const char* cfg_index_files[] = 
{
    "index.html",
    "Index.html",
    "default.htm",
    // add your default index files here
    NULL    // terminator
};


// configurable parameters

string  cfg_server_name;
string  cfg_document_root;
int     cfg_port = DEF_HTTP_PORT;
bool    cfg_syslog = true;
bool    cfg_dir_indexes = false;
bool    cfg_def_index_files = true;
bool    cfg_log_referer = true;
bool    cfg_file_mtime = true;
bool    cfg_daemonize = false;
string  cfg_user;
string  cfg_group;
string  cfg_log_file;
int     cfg_max_clients = DEF_MAX_CLIENTS;
int     cfg_max_keep_alive = (DEF_MAX_CLIENTS * 2) / 3;

char* myname = "wshare";


static void usage()
{
    static const char* usage_str = 
"%s, a simple HTTP daemon.\n\n"
"usage: %s [options] document-root\n\n"
"  -D              daemonize, UNIX only\n"
"  -d              allow directory indexes\n"
"  -g group        group ID to run as, UNIX only\n"
"  -n num          maximum number of simultaneous connections (default: %d)\n"
"  -o file-name    write HTTP access log to file-name\n"
"  -p port-num     port number to listen to\n"
"  -u user         user ID to run as, UNIX only\n"
"  -x              always show directory indexes (ignore default index files)\n"
"\n";

    printf(usage_str, pconst(SERVER_APP_NAME), myname, DEF_MAX_CLIENTS);
    exit(255);
}


static int arg_count;
static char** args;
static char* arg_ptr;
static int arg_num;


static char* arg_param()
{
    arg_ptr++;
    if (*arg_ptr == 0)
    {
        if (++arg_num >= arg_count)
            usage();
        arg_ptr = args[arg_num];
    }
    char* res = arg_ptr;
    arg_ptr += strlen(res);
    return res;
}


void config_init(int argc, char* argv[])
{
    arg_count = argc;
    args = argv;

    if (arg_count == 1)
        usage();

    arg_num = 1;
    while (arg_num < arg_count)
    {
        arg_ptr = args[arg_num];
        if (*arg_ptr != '-')
        {
            cfg_document_root = arg_ptr;
            arg_num++;
            continue;
        }

        arg_ptr++;

opt2:
        switch(*arg_ptr)
        {
        case 'D': cfg_daemonize = true; arg_ptr++; break;
        case 'd': cfg_dir_indexes = true; arg_ptr++; break;
        case 'n': 
            cfg_max_clients = atoi(arg_param()); 
            cfg_max_keep_alive = (cfg_max_clients * 2) / 3;
            break;
        case 'o': cfg_log_file = arg_param(); break;
        case 'p': cfg_port = atoi(arg_param()); break;
        case 'x': cfg_def_index_files = false; cfg_dir_indexes = true; arg_ptr++; break;
        case 'u': cfg_user = arg_param(); break;
        case 'g': cfg_group = arg_param(); break;
        default: usage(); break;
        }
        
        if (*arg_ptr != 0)
            goto opt2;

        arg_num++;
    }

    if (isempty(cfg_document_root))
        usage();
}


void config_done()
{
}

