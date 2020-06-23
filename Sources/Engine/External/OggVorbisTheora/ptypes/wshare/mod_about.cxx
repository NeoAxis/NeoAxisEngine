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


//
// a sample path handler that responds to http://hostname/.about
// see modules.h and request.h for details
//

void handle_about(request_rec& req)
{
    req.keep_alive = false; // we don't know the content length

    // all responses must start with begin_response() and end with end_response()
    req.begin_response(200, "OK");

    // use put_xxx functions (see request.h) to send response headers back to the
    // client. these functions do nothing if the request version was HTTP/0.9
    req.put_content_type("text/html");

    // end_headers() must be called when you're done with the headers. 
    // if the method was HEAD, it throws an ehttp exception so that the
    // rest of your code won't be executed
    req.end_headers();

    std_html_header(*req.sockout, "about wshare");

    // you can write to the client socket using req.sockout, which is an outstm object
    req.sockout->putf("<p>%s<br>\n"
        "<a href=\"http://www.melikyan.com/ptypes/\">PTypes</a> (C++ Portable Types Library)\n"
        "demo program<br>\n"
        "written by Hovik Melikyan</p>\n", SERVER_APP_NAME);

    std_html_footer(*req.sockout);

    // end_response() throws an ehttp exception. the request info is being logged.
    req.end_response();
}
