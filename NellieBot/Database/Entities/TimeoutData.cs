using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NellieBot.Database.Entities
{
  public class TimeoutData
  {
    public int Id { get; set; }
    public ulong UserId { get; set; }
    public string Reason { get; set; }
    public DateTime DateTime { get; set; }
    public DateTime Until { get; set; }
  }
}
