using EBM2x.Models.config;
using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Datafile.env
{
    public class EnvironmentWriter : DatafileService
    {
        public static bool write(EnvironmentNode node)
        {
            EnvPosNodeService.SaveEnvPosNode(node.EnvPosNode);
            EnvFunctionNodeService.SaveEnvFunctionNode(node.EnvFunctionNode);

            return true;
        }
    }
}
