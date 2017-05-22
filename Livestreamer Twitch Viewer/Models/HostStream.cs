using LivestreamerTwitchViewer.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchCSharp.Models;

namespace LivestreamerTwitchViewer.Models
{
    class HostStream : IEquatable<HostStream>
    {
        private string m_hostLogin;
        private Task<StreamResult> m_streamResult;

        public string HostLogin { get { return m_hostLogin; } }
        public Stream Stream { get { return m_streamResult.Result.Stream; } }
        public Task<StreamResult> StreamResult { get { return m_streamResult; } }

        public HostStream(Host p_host)
        {
            m_hostLogin = p_host.Hosts.HostLogin;
            if (p_host.Hosts.TargetLogin == null || p_host.Hosts.TargetLogin == String.Empty)
            {
                m_streamResult = null;
            }
            else
            {
                AuthenticatedClient.stack++;
                //m_streamResult = Globals.AClient.GetStreamAsync(p_host.Hosts.TargetLogin);
                m_streamResult = Globals.AClient.GetStreamAsyncV5(p_host.Hosts.TargetId.ToString());
            }
        }

        public bool Equals(HostStream other)
        {
            throw new NotImplementedException();
        }
    }
}
