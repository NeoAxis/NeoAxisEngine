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

#include <ptypes.h>
#include <pinet.h>

#include "urlutils.h"


USING_PTYPES


char* opt_anonymous_username = "ftp";
char* opt_anonymous_password = "unknown@";
char* opt_default_urlscheme = "http";


static cset schemechars("0-9A-Za-z~-.");

// TODO: make these sets RFC2396-compliant
static cset unsafechars("~00-~20%:/@?#;\\<>+\"'~7F-~FF");
static cset unsafepathchars = unsafechars - '/';
static cset unsafeparamchars = unsafechars;
static cset pathtermchars("~00#?;");


bool isurl(const string& s)
{
    const char* p = pconst(s);
    const char* b = p;
    while (*p & schemechars)
        p++;
    return p > b && *p == ':';
}


static string urlencode(const string& s, const cset& unsafe)
{
    static const char hexchars[17] = "0123456789ABCDEF";

    int numunsafe = 0;
    const char* p;
    for (p = s; *p != 0; p++)
        if (*p & unsafe)
            numunsafe++;
    if (numunsafe == 0)
        return s;

    string ret;
    setlength(ret, length(s) + numunsafe * 2);
    p = s;
    char* d = unique(ret);
    for (; *p != 0; p++, d++)
    {
        if (*p & unsafe)
        {
            *d++ = '%';
            *d++ = hexchars[*p >> 4];
            *d = hexchars[*p & 0x0f];
        }
        else
            *d = *p;
    }
    return ret;
}


string urlencodepath(const string& path)
{
    return urlencode(path, unsafepathchars);
}


static int urldefport(const string& scheme)
{
    if (scheme == "http")
        return 80;
    else if (scheme == "https")
        return 443;
    else if (scheme == "ftp")
        return 21;
    else
        return 0;
}


urlrec::urlrec()
    : scheme(), username(), password(), pwdset(false), host(),
      port(0), path(), proto(), query(), fragment() {}


void urlclear(urlrec& u)
{
    clear(u.scheme);
    clear(u.username);
    clear(u.password);
    u.pwdset = false;
    clear(u.host);
    u.port = 0;
    clear(u.path);
    clear(u.proto);
    clear(u.query);
    clear(u.fragment);
}


// urlcreate:
// NOTES: if scheme is empty, opt_default_urlscheme is taken; for ftp scheme 
// leading '/' in the path component is significant; for all other schemes 
// the leading '/' is ignored.

string urlcreate(const urlrec& u)
{
    string ret;
    string s, p;

    // scheme
    if (isempty(u.scheme))
        s = opt_default_urlscheme;
    else
        s = lowercase(u.scheme);
    if (s[length(s) - 1] == ':')
        setlength(s, length(s) - 1);
    ret = s + "://";

    // username and password
    if (!isempty(u.username))
    {
        ret += urlencode(u.username, unsafechars);
        if (u.pwdset)
            ret += ':' + urlencode(u.password, unsafechars);
        ret += '@';
    }

    // host name and port number
    ret += u.host;
    if (u.port != 0 && u.port != urldefport(s))
        ret += ':' + itostring(u.port);

    // path
    ret += '/';
    p = u.path;
    if (!isempty(p) && p[0] == '/')
    {
        if (s == "ftp")
            ret += "%2F";
        del(p, 0, 1);
    }
    ret += urlencode(p, unsafepathchars);

    // params
    if (!isempty(u.proto))
        ret += ';' + urlencode(u.proto, unsafeparamchars);
    if (!isempty(u.query))
        ret += '?' + urlencode(u.query, unsafeparamchars);
    if (!isempty(u.fragment))
        ret += '#' + urlencode(u.fragment, unsafeparamchars);

    return ret;
}


// urlcrack:
// Note: no "friendly" URL's! If the URL is invalid the result is undefined.

static int xchartoint(char c)
{
    if (c >= 'a')
        return c - 'a' + 10;
    else if (c >= 'A')
        return c - 'A' + 10;
    else
        return c - '0';
}


static void assignurlstr(string& s, const char* p, const char* end, bool decodeplus = false)
{
    if (p >= end)
        clear(s);
    else
    {
        setlength(s, end - p);
        char* d = unique(s);
        for (; p < end; p++, d++)
        {
            if (*p == '+' && decodeplus)
                *d = ' ';
            else if (*p == '%')
            {
                if (++p == end) break;
                *d = char(xchartoint(*p) << 4);
                if (++p == end) break;
                *d |= char(xchartoint(*p));
            }
            else
                *d = *p;
        }
        setlength(s, d - pconst(s));
    }
}


static void crackpath(const char* p, urlrec& u)
{
    const char* end = p;
    while (!(*end & pathtermchars))  // [#0, '?', '#', ';']
        end++;
    assignurlstr(u.path, p, end);

    // leading '/'
    if (u.scheme == "http" || u.scheme == "https")
    {
        if (isempty(u.path))
            u.path = '/';
        else if (*pconst(u.path) != '/' && *pconst(u.path) != '~')
            ins('/', u.path, 0);
    }

    // parameters
    while (*end != 0)
    {
        char paramtype = *end;
        p = ++end;
        while (!(*end & pathtermchars))
            end++;
        switch(paramtype)
        {
        case ';':
            assignurlstr(u.proto, p, end);
            break;
        case '?':
            assignurlstr(u.query, p, end, true);
            break;
        case '#':
            assignurlstr(u.fragment, p, end);
            break;
        }
    }
}


void urlcrack(const string& s, urlrec& u)
{
    urlclear(u);

    const char* t;
    const char* p = s;
    const char* end = p;

    // scheme
    while (*end & schemechars)
        end++;
    if (*end != ':' || *(end + 1) != '/' || *(end + 2) != '/')
        return;   // invalid scheme: we don't want to generate errors
    assignurlstr(u.scheme, p, end);
    u.scheme = lowercase(u.scheme);
    end += 3;  // skip '://'

    // hostname and possibly username:password
    p = end;
    while (*end != 0 && *end != '/' && *end != '@')
        end++;

    // username and possibly password
    if (*end == '@')
    {
        t = p;
        while (*t != ':' && *t != '@')
            t++;
        // password
        if (*t == ':')
        {
            assignurlstr(u.password, t + 1, end);
            u.pwdset = true;
        }
        // username
        assignurlstr(u.username, p, t);
        end++;
        p = end;
        while (*end != 0 && *end != '/')
            end++;
    }
    else if (u.scheme == "ftp")
    {
        u.username = opt_anonymous_username;
        u.password = opt_anonymous_password;
        u.pwdset = true;
    }

    // hostname and possibly port number
    t = p;
    while (*t != 0 && *t != ':' && *t != '/')
        t++;
    assign(u.host, p, t - p);

    // port
    if (*t == ':')
    {
        string s;
        assign(s, t + 1, end - t - 1);
        u.port = atoi(s);
    }

    // path
    if (*end == '/')
        end++;
    crackpath(end, u);
}


