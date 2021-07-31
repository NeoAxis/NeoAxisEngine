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

#include <sys/types.h>
#include <sys/stat.h>
#include <stdlib.h>
#include <string.h>

#ifdef WIN32
#  include <windows.h>
#  include <io.h>
#  include <direct.h>
#else
#  include <fcntl.h>
#  include <unistd.h>
#  include <sys/utsname.h>
#  include <dirent.h>
#  include <limits.h>
#  include <sys/param.h>
#  include <pwd.h>
#  include <grp.h>
#endif

#include <pport.h>

#include "sysutils.h"
#include "utils.h"


USING_PTYPES


void throw_msg(const string& msg)
{
    throw new exception(msg);
}


static char* nodename = 0;
static char* username = 0;


char* get_nodename()
{
    if (nodename == 0)
    {
#ifdef WIN32
        char buf[MAX_COMPUTERNAME_LENGTH + 1];
        unsigned long bufsize = sizeof(buf);
        if (!GetComputerName(buf, &bufsize))
            fatal(1001, "GetComputerName() failed");
        nodename = strdup(buf);
#else
	struct utsname u;
	if (uname(&u) < 0)
	    fatal(1001, "Couldn't get the node name");
	nodename = strdup(u.nodename);
#endif
    }
    return nodename;
}


char* get_username()
{
    if (username == 0)
    {
#ifdef WIN32
        char buf[256 + 1];
        unsigned long bufsize = sizeof(buf);
        if (!GetUserName(buf, &bufsize))
            fatal(1002, "GetUserName() failed");
        username = strdup(buf);
#else
        struct passwd* pw = getpwuid(getuid());
        if (pw == 0)
	    fatal(1001, "Couldn't get the user name");
        username = strdup(pw->pw_name);
#endif
    }
    return username;
}


#ifdef WIN32
int get_user_id(const char*)
{
    return 0;
#else
int get_user_id(const char* user_name)
{
    struct passwd* pw = getpwnam(user_name);
    if (pw == 0)
        return -1;
    else
        return pw->pw_uid;
#endif
}


#ifdef WIN32
int get_user_gid(const char*)
{
    return 0;
#else
int get_user_gid(const char* user_name)
{
    struct passwd* pw = getpwnam(user_name);
    if (pw == 0)
        return -1;
    else
        return pw->pw_gid;
#endif
}


#ifdef WIN32
int get_group_id(const char*)
{
    return 0;
#else
int get_group_id(const char* group_name)
{
    struct group* gr = getgrnam(group_name);
    if (gr == 0)
        return -1;
    else
        return gr->gr_gid;
#endif
}


#ifdef WIN32
string get_group_name(int)
{
    return nullstring;
#else
string get_group_name(int gid)
{
    struct group* gr = getgrgid(gid);
    if (gr == 0)
        return nullstring;
    else
        return gr->gr_name;
#endif
}


file_type_t get_file_type(const char* name)
{
    struct stat st;
    if (stat(name, &st) != 0)
        return FT_ERROR;
    if ((st.st_mode & S_IFDIR) == S_IFDIR)
        return FT_DIRECTORY;
    if ((st.st_mode & S_IFREG) == S_IFREG)
        return FT_FILE;
    return FT_OTHER;
}


#ifdef WIN32
bool is_symlink(const char*)
{
    return false;
#else
bool is_symlink(const char* name)
{
    struct stat st;
    if (lstat(name, &st) != 0)
        return false;
    return (st.st_mode & S_IFLNK) == S_IFLNK;
#endif
}



#ifdef WIN32
bool is_executable(const char* name)
{
    return stricmp(get_file_ext(name), ".exe") == 0;
#else
bool is_executable(const char* name)
{
    struct stat st;
    if (stat(name, &st) != 0)
        return false;
    return (st.st_mode & S_IXOTH) == S_IXOTH;
#endif
}



large get_file_size(const char* name)
{
#ifdef WIN32
	struct __stat64 st;
    if (_stat64(name, &st) != 0)
#else
	struct stat st;
    if (stat(name, &st) != 0)
#endif
        return -1;
    return st.st_size;
}


datetime get_file_mtime(const char* name)
{
    struct stat st;
    if (stat(name, &st) != 0)
        return invdatetime;
    return utodatetime(st.st_mtime);
}


#if defined(__sun__) || defined(__hpux)


int daemon(int nochdir, int noclose) {
    int fd;
    
    switch (fork()) 
    {
    case -1:
        return -1;
    case 0:
        break;
    default:
        exit(0);
    }
    
    if (setsid() == -1)
        return -1;
    
    if (!nochdir)
        chdir("/");
    
    if (!noclose && (fd = open("/dev/null", O_RDWR, 0)) != -1) 
    {
        dup2(fd, STDIN_FILENO);
        dup2(fd, STDOUT_FILENO);
        dup2(fd, STDERR_FILENO);
        if (fd > 2)
            close(fd);
    }
    return 0;
}


#elif defined WIN32


int daemon(int nochdir, int)
{
    if (!nochdir)
        chdir("/");
    return 0;
}


#endif


#ifdef WIN32
void downgrade(const string&, string&)
{
#else
void downgrade(const string& user, string& group)
{
    int uid = get_user_id(user);
    if (uid < 0)
        throw_msg("Unknown user: " + user);

    int gid;
    if (isempty(group))
    {
        gid = get_user_gid(user);
        group = get_group_name(gid);
    }
    else
        gid = get_group_id(group);

    if (gid < 0)
        throw_msg("Unknown group: " + group);

    if (setgid(gid) < 0)
        throw_msg("Couldn't change effective user/group ID (not root?)");
    if (initgroups(user, gid))
        throw_msg("initgroups() failed"); // what else can we say here?
    if (setuid(uid) < 0)
        throw_msg("Couldn't change effective user ID (not root?)");
#endif
}


file_info::file_info(const char* iname, large isize, datetime imodified)
    : name(iname), size(isize), modified(imodified)  {}


void get_directory(filist& s, string path, bool dirs, int maxfiles)
{
#ifdef WIN32
    path += "*.*";
    _finddatai64_t f;
    int h = _findfirsti64((char*)pconst(path), &f); // this stupid typecast is for BCC
    if (h < 0)
        return;
    do
    {
        if (((f.attrib & _A_SUBDIR) != 0) == dirs)
        {
            string t = f.name;
            if (t == '.')
                continue;
            if (dirs)
                t += '/';
            s.add(t, new file_info(t, f.size, utodatetime(f.time_write)));
        }
    }
    while (_findnexti64(h, &f) == 0 && s.get_count() < maxfiles);
    _findclose(h);

#else
    if (path != '/' && trail_char(path) == '/')
        trunc_trail_char(path);
    DIR* dir = opendir(path);
    if (dir == 0)
        return;
    dirent* de;
    while ((de = readdir(dir)) != 0 && s.get_count() < maxfiles)
    {
        string name = de->d_name;
        if (name == '.')
            continue;
        string fullname = path + '/' + name;

#if defined(__sun__) || defined(__hpux) || defined(__CYGWIN__)
        file_type_t ft = get_file_type(fullname);
        if ((ft == FT_DIRECTORY) != dirs)
            continue;
        if (ft != FT_DIRECTORY && ft != FT_FILE && !is_symlink(fullname))
            continue;
#else
        if ((de->d_type == DT_DIR) != dirs)
            continue;
        if (de->d_type != DT_DIR && de->d_type != DT_REG && de->d_type != DT_LNK)
            continue;
#endif

        large size = 0;
        if (dirs)
            name += '/';
        else
            size = get_file_size(fullname);
        datetime mtime = get_file_mtime(fullname);
        s.add(name, new file_info(name, size, mtime));
    }
    closedir(dir);
#endif
}


string absolute_path(const string& rel)
{
    string abs;

#ifdef WIN32
    setlength(abs, 4096);
    char* p = unique(abs);
    _fullpath(p, rel, 4096);
    while (*p != 0)
    {
        if (*p == '\\')
            *p = '/';
        p++;
    }
#else
#  ifdef PATH_MAX
    int path_max = PATH_MAX;
#  else
    int path_max = pathconf(path, _PC_PATH_MAX);
	if (path_max <= 0)
        path_max = 4096;
#  endif
    setlength(abs, path_max);
    char* p = unique(abs);
    realpath(rel, p);
#endif

    return pconst(abs);
}


string get_file_ext(const string& name)
{
    const char* e = pconst(name) + length(name);
    int len = 0;
    while (e > pconst(name) && *e != '.' && *e != '/')
    {
        e--;
        len++;
    }
    if (*e == '.' && len > 1)
        return string(e, len);
    else
        return emptystr;
}


