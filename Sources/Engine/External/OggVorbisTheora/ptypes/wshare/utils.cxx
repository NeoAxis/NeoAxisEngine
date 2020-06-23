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

#include <ptypes.h>
#include <ptime.h>

#include "config.h"
#include "sysutils.h"
#include "utils.h"
#include "urlutils.h"


USING_PTYPES


char trail_char(const string& s)
{
    if (isempty(s))
        return 0;
    else
        return *(pconst(s) + length(s) - 1);
}


void trunc_trail_char(string& s)
{
    if (isempty(s))
        return;
    else
        setlength(s, length(s) - 1);
}



static const cset htchars = "<>&\"~00";

void html_encode(outstm& s, const char* p)
{
    while (*p != 0)
    {
        const char* b = p;
        while (!(*p & htchars))
            p++;
        s.write(b, p - b);
        switch (*p)
        {
            case '<': s.put("&lt;"); break;
            case '>': s.put("&gt;"); break;
            case '&': s.put("&amp;"); break;
            case '"': s.put("&quot;"); break;
            case '\xa0': s.put("&nbsp;"); break;
        }
        if (*p != 0)
            p++;
    }
}


void std_html_header(outstm& s, const string& title)
{
    s.put("<html><head>\n");
    s.put("<title>");
    html_encode(s, title);
    s.put("</title>\n");
    s.put(STD_CSS);
    s.put("</head><body>\n");
    s.put("<h3><br>");
    html_encode(s, title);
    s.put("</h3>\n");
}


void std_html_footer(outstm& s)
{
    s.put("<p><hr noshade size=1>");
    s.put(SERVER_APP_NAME);
    s.put(" at ");
    s.put(cfg_server_name);
    if (cfg_port != DEF_HTTP_PORT)
        s.put(':' + itostring(cfg_port));
    s.put("</p>\n</body></html>\n");
}


void html_show_file_list(outstm& s, const filist& list)
{
    const int FSIZE_WIDTH = 5;

    for (int i = 0; i < list.get_count(); i++)
    {
        file_info* f = list[i];

        s.put(dttostring(f->modified, "%d-%b-%Y  %H:%M"));
        s.put("  ");
        
        if (trail_char(f->name) != '/')
        {
            string t = itostring(f->size);
            char c = ' ';
            if (length(t) > FSIZE_WIDTH)
            {
                c = 'k';
                t = itostring(f->size / 1024);
                if (length(t) > FSIZE_WIDTH)
                {
                    c = 'M';
                    t = itostring(f->size / 1024 / 1024);
                }
            }

            s.put(pad(t, FSIZE_WIDTH, ' ', false));
            s.put(c);
        }
        else
            s.put("    - ");
        
        s.put("  ");
        s.put("<a href=\"");
        html_encode(s, urlencodepath(f->name));
        s.put("\">");
        html_encode(s, f->name);
        s.put("</a>\r\n");
    }
}


//
// splits a URI path into components and builds
// a list of directory names. also resolves './'
// and '../' references
//
void split_path(const char* path, strlist& list)
{
    list.clear();
    const char* e = path;
    if (*e == '/')
        e++;
    const char* b = e;
    while (*b != 0)
    {
        e = strchr(e, '/');
        if (e == nil)
            e = path + strlen(path);
        if (e > b)
        {
            string s(b, e - b);                 // directory name
            if (s != '.')                       // ignore './' self-references
            {
                if (s == "..")                  // resolve '../' references
                {
                    if (list.get_count() > 0)
                        list.del(list.get_count() - 1);
                }
                else
                    list.add(s, nil);
            }
        }
        if (*e == '/')
            e++;
        b = e;
    }
}


string get_mimetype(const string& path)
{
    string ext = get_file_ext(path);
    if (isempty(ext))
        if (is_executable(path))
            return "application/octet-stream";
        else
            return DEF_MIME_TYPE;

    const char** p = mimetypes;
    while (*p != 0)
    {
        if (**p == '.' && ext == *p)
        {
            do {
                p++;
            } while (**p == '.');
            return *p;
        }
        p++;
    }

    return "application/octet-stream";
}


const cset digits = "0-9";
const cset letters = "A-Za-z";
const cset non_date_chars = cset("~20-~FF") - digits - letters;

static const char* mnames[12] = 
    {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};
static const char* downames[7] = 
    {"Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"};

datetime parse_http_date(const string& d)
{
    string s;
    inmemory m(d);
    m.open();

    m.skiptoken(letters);   // day of week
    m.skiptoken(non_date_chars);

    s = m.token(digits);    // day
    if (length(s) == 0)
        return invdatetime;
    int day = atoi(s);
    m.skiptoken(non_date_chars);

    s = m.token(letters);   // month
    setlength(s, 3);
    int month = 0;
    for (int i = 0; i < 12; i++)
    {
        if (s == mnames[i])
        {
            month = i + 1;
            break;
        }
    }
    if (month == 0)
        return invdatetime;
    m.skiptoken(non_date_chars);

    s = m.token(digits);   // year
    if (length(s) == 0)
        return invdatetime;
    int year = atoi(s);
    if (year < 50)
        year += 2000;
    else if (year < 100)
        year += 1900;
    m.skiptoken(non_date_chars);

    int hour = atoi(m.token(digits));
    m.get();
    int min = atoi(m.token(digits));
    m.get();
    int sec = atoi(m.token(digits));

    return encodedate(year, month, day) + encodetime(hour, min, sec);
}


// #define HTTP_DATE_FMT           "%a, %d %b %Y %H:%M:%S GMT"

string http_time_stamp(datetime t)
{
    if (t == invdatetime)
        t = now(true);

    int dow, year, month, day, hour, min, sec;
    decodedate(t, year, month, day);
    decodetime(t, hour, min, sec);
    dow = dayofweek(t);

    char buf[128];
    snprintf(buf, sizeof(buf), "%s, %02d %s %04d %02d:%02d:%02d GMT", 
        downames[dow], day, mnames[month - 1], year, hour, min, sec);

    return buf;
}

