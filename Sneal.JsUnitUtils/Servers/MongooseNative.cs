#region license
// Copyright 2008 Shawn Neal (sneal@sneal.net)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Runtime.InteropServices;

namespace Sneal.JsUnitUtils.Servers
{
    /// <summary>
    /// Wraps the native Mongoose.dll 
    /// </summary>
    public static class MongooseNative
    {
        private const string MongooseDll = "mongoose.dll";

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct mg_http_header
        {
            public string name;
            public string value;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct mg_request_info
        {
            public string request_method;
            public string uri;
            public string query_string;
            public string post_data;
            public string remote_user;
            public int remote_ip;
            public int remote_port;
            public int post_data_len;
            public int http_version_minor;
            public int http_version_major;
            public int status_code;
            public int num_headers;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public mg_http_header http_headers;
        }

        [DllImport(MongooseDll)]
        public static extern IntPtr mg_start();

        [DllImport(MongooseDll)]
        public static extern int mg_set_option(IntPtr ctx, string opt_name, string opt_value);

        [DllImport(MongooseDll)]
        public static extern void mg_bind_to_uri(IntPtr ctx, string uri_regex, mg_callback_t func);

        [DllImport(MongooseDll)]
        public static extern int mg_write(IntPtr conn, string fmt, int len);

        public delegate void mg_callback_t(IntPtr ctx, mg_request_info request_info);
    }
}
