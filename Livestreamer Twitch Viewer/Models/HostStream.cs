using LivestreamerTwitchViewer.Client;
using System;
using TwitchCSharp.Models;

namespace LivestreamerTwitchViewer.Models
{
    class HostStream
    {
        private string m_hostLogin;
        private Stream m_stream;

        public string HostLogin { get { return m_hostLogin; } }
        public Stream Stream { get { return m_stream; } }

        public HostStream(Host p_host, Stream p_stream)
        {
            m_hostLogin = p_host.Hosts.HostLogin;
            if (p_host.Hosts.TargetLogin == null || p_host.Hosts.TargetLogin == String.Empty || p_stream == null)
            {
                m_stream = null;
            }
            else
            {
                m_stream = p_stream;
            }
        }
    }
}
