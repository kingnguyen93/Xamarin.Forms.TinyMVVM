using System;

namespace TinyMVVM
{
    public class MenuItem
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Icon { get; set; }

        public Type TargetType { get; set; }
    }
}