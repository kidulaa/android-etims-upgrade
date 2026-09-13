using EBM2x.Models.config;

namespace EBM2x.Datafile.env
{
    public class EnvironmentReader : DatafileService
    {
        public static bool IsExist()
        {
            return EnvPosNodeService.IsEnvPosNode();
        }

        public static bool read(EnvironmentNode node)
        {
            node.EnvPosSetup = EnvPosSetupService.LoadEnvPosSetup();
            node.EnvPosNode = EnvPosNodeService.LoadEnvPosNode();
            node.EnvFunctionNode = EnvFunctionNodeService.LoadEnvFunctionNode();

            return true;
        }
    }
}
