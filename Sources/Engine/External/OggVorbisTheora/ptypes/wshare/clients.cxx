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

#include <pasync.h>

#include "config.h"
#include "log.h"
#include "clients.h"


USING_PTYPES


thread_list  threads;
int          thread_count;
int          thread_seq;
datetime     started;


thread_list::thread_list(): lock(), count(0), list(0)
{
}


thread_list::~thread_list()  
{
    memfree(list);
}


void thread_list::set_capacity(int icount)
{
    if (list != 0)
        fatal(1001, "");
    count = icount;
    list = (client_thread**)memalloc(sizeof(client_thread*) * count);
    memset(list, 0, sizeof(client_thread*) * count);
}


void thread_list::add(client_thread* t)
{
    scopewrite _lock(lock);
    int i;
    for (i = 0; i < count; i++)
        if (list[i] == 0)
            break;
    if (i == count)
        fatal(1002, "");
    list[i] = t;
}


void thread_list::del(client_thread* t)
{
    scopewrite _lock(lock);
    int i;
    for (i = 0; i < count; i++)
        if (list[i] == t)
            break;
    if (i == count)
        fatal(1003, "");
    list[i] = 0;
}


client_thread::client_thread(ipstream* iclient)
    : thread(true),
      request_rec(*iclient, *iclient, iclient->get_ip()),
      client(iclient), seq_num(0)
{
    pincrement(&thread_count);
}


void client_thread::cleanup()
{
    delete client;
    threads.del(this);
}


client_thread::~client_thread()
{
    pdecrement(&thread_count);
}


void client_thread::execute()
{
    seq_num = pincrement(&thread_seq);
    threads.add(this);

    try
    {
        while (1)
        {
            request_rec::respond();
            if (!client->get_active())
                break;
            if (!client->waitfor(DEF_KEEP_ALIVE_TIMEOUT))
                break;
            if (client->get_eof())
                break;
            reset_state();
        }
    }
    catch(estream* e)
    {
        htlog_write(client_ip, req_line, rsp_code, sockout->tellx() - hdr_size, referer);
        client->close();
        delete e;
    }
    
    client->close();
}


