using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace pixit.Client.Shared
{
    public partial class Avatar
    {
        [Parameter] public bool IsHost { get; set; }
        [Parameter] public int Size { get; set; }
        [Parameter] public int Gender { get; set; }
        [Parameter] public int Background { get; set; }
        [Parameter] public int Face { get; set; }
        [Parameter] public int Cloth { get; set; }
        [Parameter] public int Hair { get; set; }
        [Parameter] public int Mouth { get; set; }
        [Parameter] public int Eye { get; set; }

        public string GetBackground()
        {
            return (-1) * Background * Size + "px 0";
        }

        public string GetPositionOneCol(int param)
        {
            return (-1) * param * Size + "px " + (-1) * Gender * Size + "px";
        }

        public string GetEye()
        {
            var col = (Eye >= 25 ? 1 : 0);
            return (-1) * (Eye % 25) * Size + "px " + (-1) * (Gender * 2 + col) * Size * 0.6 + "px";
        }
        
        public string GetCloth()
        {
            var col = (Cloth >= 25 ? 1 : 0);
            return (-1) * (Cloth % 25) * Size + "px " + (-1) * (Gender * 2 + col) * Size * 0.35 + "px";
        }
    }
}