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


#ifndef W_URLUTILS_H
#define W_URLUTILS_H


#include <pport.h>
#include <ptypes.h>


USING_PTYPES


//
//  URL utilities
//
//  URL ::= <scheme>://[<authinfo>][<host>[:<port>]]/[<url-path>][<extra>]
//  <authinfo> ::= <user>[:<password>]@
//  <extra> ::=  [ ;<protocol-params> ] [ ?<query-params> ] [ #<fragment> ]
//


struct urlrec {
    string scheme;      // 'http', 'ftp', 'file', ...
    string username;    // for ftp scheme defaults to opt_anonymous_username
    string password;    // for ftp scheme defaults to opt_anonymous_password
    bool   pwdset;      // empty password and NO password is not the same!
    string host;        // Internet or NetBIOS host name
    int    port;        // 0 = default for the given scheme
    string path;        // be careful with the leading '/' (RFC1738)
    string proto;       // ';'
    string query;       // '?'
    string fragment;    // '#'

    urlrec();
};


extern char* opt_anonymous_username;
extern char* opt_anonymous_password;
extern char* opt_default_urlscheme;

bool   isurl(const string& s);
void   urlclear(urlrec& u);
string urlencodepath(const string& path);
string urlcreate(const urlrec& u);
void   urlcrack(const string& s, urlrec& u);


#endif
