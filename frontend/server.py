#!/usr/bin/env python3
import os
import sys
from http.server import HTTPServer, SimpleHTTPRequestHandler

class SPAHandler(SimpleHTTPRequestHandler):
    def do_GET(self):
        if self.path != '/' and not os.path.exists(f'dist/frontend/browser{self.path}'):
            self.path = '/index.html'
        return super().do_GET()

    def end_headers(self):
        self.send_header('Cache-Control', 'no-store, no-cache, must-revalidate')
        super().end_headers()

os.chdir('dist/frontend/browser')
server = HTTPServer(('127.0.0.1', 8080), SPAHandler)
print('Server running at http://127.0.0.1:8080')
server.serve_forever()
