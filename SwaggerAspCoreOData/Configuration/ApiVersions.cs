﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SwaggerAspCoreOData.Configuration
{
  static class ApiVersions
  {
    internal static readonly ApiVersion V1 = new ApiVersion(1, 0);
    internal static readonly ApiVersion V2 = new ApiVersion(2, 0);
    internal static readonly ApiVersion V3 = new ApiVersion(3, 0);
  }
}
