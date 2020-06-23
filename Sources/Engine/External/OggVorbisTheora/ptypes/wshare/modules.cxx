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

#include "request.h"
#include "modules.h"


USING_PTYPES


static handler_info* method_list;
static handler_info* path_list;
static handler_info* file_list;


handler_info::handler_info(handler_info* inext, void* icallback, const string& iparam)
    : next(inext), callback(icallback), param(iparam)  
{
}


void add_method_handler(const string& method, method_callback cb)
{
    method_list = new handler_info(method_list, (void*)cb, method);
}


void add_path_handler(const string& path, path_callback cb)
{
    path_list = new handler_info(path_list, (void*)cb, path);
}



void add_file_handler(const string& ext, file_callback cb)
{
    file_list = new handler_info(file_list, (void*)cb, ext);
}


handler_info* find_handler(handler_info* list, const string& param)
{
    while (list != 0)
    {
        if (list->param == param)
            return list;
        list = list->next;
    }
    return 0;
}


handler_info* find_method_handler(const string& method)
{
    return find_handler(method_list, method);
}


handler_info* find_path_handler(const string& path)
{
    return find_handler(path_list, path);
}


handler_info* find_file_handler(const string& ext)
{
    return find_handler(file_list, ext);
}


void init_handlers()
{
    ADD_PATH_HANDLER("/", handle_file);
    ADD_PATH_HANDLER("/.about", handle_about);
    ADD_PATH_HANDLER("/.wstat", handle_wstat);
}

