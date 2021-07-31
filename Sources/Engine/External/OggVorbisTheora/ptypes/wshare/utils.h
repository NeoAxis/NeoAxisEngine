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


#ifndef W_UTILS_H
#define W_UTILS_H

#include <ptypes.h>
#include <pstreams.h>
#include <ptime.h>


#ifndef W_SYSUTILS_H
#  include "sysutils.h"
#endif


USING_PTYPES


char      trail_char(const string& s);
void      trunc_trail_char(string& s);
void      html_encode(outstm& s, const char* p);
void      std_html_header(outstm& s, const string& title);
void      std_html_footer(outstm& s);
void      html_show_file_list(outstm& s, const filist& list);
void      split_path(const char* path, strlist& list);
string    get_mimetype(const string& path);
datetime  parse_http_date(const string& d);
string    http_time_stamp(datetime t);


#endif
