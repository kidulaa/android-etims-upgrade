using System;
using System.Collections.Generic;
using System.Text;

namespace EBM2x.Models.config
{

    public class EnvironmentNode
    {

        public EnvPosSetup EnvPosSetup { get; set; }            
        public EnvPosNode EnvPosNode { get; set; }              
        public EnvFunctionNode EnvFunctionNode { get; set; }    

        public EnvironmentNode()
        {
            EnvPosSetup = new EnvPosSetup();            
            EnvPosNode = new EnvPosNode();              
            EnvFunctionNode = new EnvFunctionNode();	
        }
    }
}
