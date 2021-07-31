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


#ifndef W_SYSUTILS_H
#define W_SYSUTILS_H

#ifndef WIN32
#  include <unistd.h>
#endif

#include <pport.h>
#include <ptypes.h>
#include <ptime.h>


USING_PTYPES


enum file_type_t
{
    FT_FILE, 
    FT_DIRECTORY, 
    FT_OTHER,       // device or pipe
    FT_ERROR = -1
};


class file_info
{
public:
    string     name;
    large      size;
    datetime   modified;
    file_info(const char* iname, large isize, datetime imodified);
};


typedef tstrlist<file_info> filist;


char*        get_nodename();
char*        get_username();
int          get_user_id(const char* user_name);
int          get_user_gid(const char* user_name);
int          get_group_id(const char* group_name);
string       get_group_name(int gid);

file_type_t  get_file_type(const char*);
bool         is_symlink(const char*);
bool         is_executable(const char*);
large        get_file_size(const char*);
datetime     get_file_mtime(const char*);
void         get_directory(filist& s, string path, bool dirs, int maxfiles);
string       absolute_path(const string& rel);
string       get_file_ext(const string& name);


inline bool is_directory(const char* name)  { return get_file_type(name) == FT_DIRECTORY; }
inline bool is_file(const char* name)       { return get_file_type(name) == FT_FILE; }


#if defined(__sun__) || defined(__hpux) || defined(WIN32)
int daemon(int nochdir, int noclose);
#endif

void downgrade(const string& user, string& group);

#endif
