using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleJSON;

namespace LivestreamerTwitchViewer.Models
{
    class Host
    {
        private List<HostRoot> m_hosts;

        public HostRoot Hosts { get { return m_hosts[0]; } }

        public Host(JSONNode node)
        {
            m_hosts = new List<HostRoot>();
            JSONArray arrHost = node["hosts"].AsArray;
            if (arrHost != null)
            {
                foreach (JSONNode n in arrHost.Children)
                {
                    m_hosts.Add(new HostRoot(n));
                }
            }
        }

        public Host()
        {
        }
    }

    class HostRoot
    {
        private double m_hostId;
        private double m_targetId;
        private String m_hostLogin;
        private string m_targetLogin;

        public double HostId { get { return m_hostId; } }
        public double TargetId { get { return m_targetId; } }
        public string HostLogin { get { return m_hostLogin; } }
        public string TargetLogin { get { return m_targetLogin; } }

        public HostRoot (JSONNode node)
        {
            m_hostId = node["host_id"].AsDouble;
            m_targetId = node["target_id"].AsDouble;
            m_hostLogin = node["host_login"].Value;
            m_targetLogin = node["target_login"].Value;
        }
    } 
}
