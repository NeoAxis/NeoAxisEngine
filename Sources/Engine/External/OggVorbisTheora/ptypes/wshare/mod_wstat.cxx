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

#include "config.h"
#include "utils.h"
#include "request.h"
#include "clients.h"


const char* stat_str[STAT_MAX] = {"READ   ", "WRITE  ", "WAIT   "};


static ipaddress localhost(127, 0, 0, 1);


static void show_lifetime(outstm* sockout, datetime t)
{
    datetime diff = now() - t;
    int d = days(diff);
    if (d > 0)
        sockout->putf("%dd ", d);
    sockout->put(dttostring(diff, "%H:%M:%S"));
}


void handle_wstat(request_rec& req)
{
    // only requests from localhost are allowed
    if (req.client_ip != localhost)
        req.rsp_forbidden();

    req.keep_alive = false;

    req.begin_response(200, "OK");
    req.put_content_type("text/html");
    req.end_headers();

    std_html_header(*req.sockout, "wshare status report");
    req.sockout->put("<pre>\n");
    req.sockout->putf("  Requests: %d\n", thread_seq);
    req.sockout->put("  Running: ");
    show_lifetime(req.sockout, ::started);
    req.sockout->put("\n\n");
    req.sockout->put("  Client          Status Lifetime  Request\n");
    req.sockout->put("<hr noshade size=1>\n");

    for (int i = 0; i < threads.count; i++)
    {
        if (threads.list[i] == 0)
            continue;

        // copy all parameters to local vars to free the thread_list mutex earlier
        ipaddress tclient_ip = 0;
        req_stat_t tstat = STAT_READ;
        datetime tstarted = invdatetime;
        string treq_line;
        {
            scoperead _lock(threads.lock);

            client_thread* t = threads.list[i];
            if (t != 0)
            {
                tclient_ip = t->client_ip;
                tstat = t->stat;
                tstarted = t->started;
                treq_line = t->req_line;
            }
        }

        if (tclient_ip == 0)
            continue;

        req.sockout->putf("  %-15s %s", pconst(iptostring(tclient_ip)), stat_str[tstat]);
        show_lifetime(req.sockout, tstarted);
        req.sockout->put("  ");
        html_encode(*req.sockout, treq_line);

        req.sockout->put("\n");
    }

    req.sockout->put("</pre>\n");
    std_html_footer(*req.sockout);

    req.end_response();
}
