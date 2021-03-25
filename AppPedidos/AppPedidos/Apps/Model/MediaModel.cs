using System;
using System.Text;
using Xamarin.Forms;
using System.Collections.Generic;

namespace AppPedidos.Apps.Model
{
    public  class MediaModel
    {
        public Guid MediaID { get; set; }
        public string Path { get; set; }
        public DateTime LocalDateTime { get; set; }

        private FileImageSource source = null;
        public FileImageSource Source => source ?? (source = new FileImageSource() { File = Path });
    }
}
