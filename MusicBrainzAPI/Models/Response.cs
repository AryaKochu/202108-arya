using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicBrainzAPI.Models
{
    public class Response
    {
        public IList<Artist> Artists{ get; set; }
        public IList<Release> Releases{ get; set; }

        public string Error { get; set; }
    }
}
