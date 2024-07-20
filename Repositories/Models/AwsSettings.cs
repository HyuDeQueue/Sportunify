using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Models
{
    public class AwsSettings
    {
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string BucketName { get; set; }
        public string Region { get; set; }
    }
}
