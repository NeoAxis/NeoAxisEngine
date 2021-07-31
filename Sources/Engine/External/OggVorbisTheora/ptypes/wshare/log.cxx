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


#include <stdio.h>
#include <stdarg.h>

#ifndef WIN32
#  include <syslog.h>
#endif

#include <ptime.h>
#include <pasync.h>

#include "config.h"
#include "log.h"


USING_PTYPES


compref<logfile> htlog;
compref<logfile> errlog;


#ifdef WIN32
#  define vsnprintf _vsnprintf
#endif


void log_init()
{
    htlog = &perr;
    errlog = &perr;
#ifndef WIN32
    if (cfg_syslog)
        openlog(myname, 0, LOG_USER);
#endif
}


void log_done()
{
#ifndef WIN32
    if (cfg_syslog)
        closelog();
#endif
}


void syslog_write(log_severity_t pri, const char* fmt, ...)
{
    char buf[2048];
    va_list va;
    va_start(va, fmt);

    vsnprintf(buf, sizeof(buf), fmt, va);

#ifndef WIN32
    if (cfg_syslog)
    {
        // syslog is available only on Unix
        static int upri[4] = {LOG_CRIT, LOG_ERR, LOG_WARNING, LOG_INFO};
        syslog(upri[pri], "%s", buf);
    }
#endif

    if (errlog != 0)
    {
        static pconst spri[4] = {"FATAL: ", "Error: ", "", ""};
        errlog->putf("%s: %t  %s%s\n", myname, now(), spri[pri], buf);
        errlog->flush();
    }

    va_end(va);
}


void htlog_write(ipaddress ip, string request, int code, large size, string referer)
{
    if (htlog == 0)
        return;

    if (isempty(referer))
        referer = '-';
    int t = tzoffset();
    bool neg = t < 0;
    if (neg)
        t = -t;
    string ssize;
    if (size < 0)
        ssize = '-';
    else
        ssize = itostring(size);

    string sdate = nowstring("%d/%b/%Y:%H:%M:%S", false);
    
    if (cfg_log_referer)
        referer = " \"" + referer + '\"';
    else
	clear(referer);

    try
    {
        htlog->putf("%a - - [%s %c%02d%02d] \"%s\" %d %s%s\n",
            long(ip), pconst(sdate), neg ? '-' : '+', t / 60, t % 60,
            pconst(request), code, pconst(ssize), pconst(referer));
        htlog->flush();
    }
    catch (estream* e)
    {
        delete e;
        htlog = 0;
        syslog_write(SYSLOG_ERROR, "HTTP log disabled due to failed write attempt (daemonized?)");
    }
}


