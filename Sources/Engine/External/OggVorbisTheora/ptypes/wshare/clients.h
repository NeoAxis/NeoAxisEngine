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


#ifndef W_CLIENTS_H
#define W_CLIENTS_H


#include <ptime.h>
#include <pasync.h>

#include "request.h"


USING_PTYPES


class client_thread;


class thread_list
{
public:
    rwlock lock;
    int count;
    client_thread** list;

    thread_list();
    virtual ~thread_list();
    void set_capacity(int icount);
    void add(client_thread* t);
    void del(client_thread* t);
};


class client_thread: public thread, public request_rec
{
protected:
    ipstream*   client;
    int         seq_num;    // sequential number, pseudo-id
    virtual void execute();
    virtual void cleanup();
public:

    client_thread(ipstream* iclient);
    virtual ~client_thread();

    int get_seq_num()  { return seq_num; }
};


extern thread_list threads;
extern int thread_count;
extern int thread_seq;
extern datetime started;


#endif
