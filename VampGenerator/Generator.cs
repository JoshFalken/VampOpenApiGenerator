using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VampGenerator
{
    class Generator
    {
        private StringBuilder ReadFile(string path)
        {
            StreamReader streamReader = new StreamReader(path);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(streamReader.ReadToEnd());
            streamReader.Close();

            return stringBuilder;
        }

        private void RemoveEmptyLines(string inputFile, string outputFile)
        {
            StreamReader sr = new StreamReader(inputFile);
            StreamWriter sw = new StreamWriter(outputFile);
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (line.Trim() != "")
                    sw.WriteLine(line);

            }
            sr.Close();
            sw.Close();
        }

        public void DoGeneration()
        {
            StreamWriter sw = new StreamWriter("pre_output.txt");

            StringBuilder template = ReadFile("template.txt");
            StringBuilder deploymentParameters = ReadFile("DeploymentParameters.txt");
            StringBuilder deploymentAdditionalParameters = ReadFile("DeploymentAdditionalParameters.txt");
            StringBuilder debugOperations = ReadFile("debug.txt");
            StringBuilder emptyParameters = new StringBuilder();
            StringBuilder definitions = ReadFile("Definitions.txt");
            StringBuilder preamble = ReadFile("Preamble.txt");

            sw.Write(preamble.ToString());
            sw.WriteLine();

            sw.Write("paths:");
            sw.WriteLine();

            AppendOperation(template, sw, "breeds", "Breed", "breed", "Breed", "#/definitions/Breed", emptyParameters, emptyParameters, emptyParameters, emptyParameters, false, false);
            AppendOperation(template, sw, "blueprints", "Blueprint", "blueprint", "Blueprint", "#/definitions/Blueprint", emptyParameters, emptyParameters, emptyParameters, emptyParameters, false, false);
            AppendOperation(template, sw, "deployments", "Deployment", "deployment", "Deployment", "#/definitions/Blueprint", deploymentParameters, emptyParameters, emptyParameters, emptyParameters, false, false);
            AppendOperation(template, sw, "gateways", "Gateway", "gateway", "Gateway", "#/definitions/Gateway", emptyParameters, emptyParameters, emptyParameters, emptyParameters, false, false);
            AppendOperation(template, sw, "scales", "Scale", "scale", "Scale", "#/definitions/Scale", emptyParameters, emptyParameters, emptyParameters, emptyParameters, false, false);
            AppendOperation(template, sw, "filters", "Filter", "filter", "Filter", "#/definitions/Filter", emptyParameters, emptyParameters, emptyParameters, emptyParameters, false, false);
            AppendOperation(template, sw, "slas", "Sla", "sla", "Sla", "#/definitions/Sla", emptyParameters, emptyParameters, emptyParameters, emptyParameters, false, false);
            AppendOperation(template, sw, "escalations", "Escalation", "escalation", "Escalation", "#/definitions/Escalation", emptyParameters, emptyParameters, emptyParameters, emptyParameters, false, false);
            AppendOperation(template, sw, "deployments/{id}/clusters/{name}/sla", "DeploymentSla", "sla part of deployment", "Deployment", "#/definitions/Sla", deploymentAdditionalParameters, deploymentAdditionalParameters, deploymentAdditionalParameters, deploymentAdditionalParameters, true, false);
            AppendOperation(template, sw, "deployments/{id}/clusters/{name}/scale", "DeploymentScale", "Scale part of deployment", "Deployment", "#/definitions/Scale", deploymentAdditionalParameters, deploymentAdditionalParameters, deploymentAdditionalParameters, deploymentAdditionalParameters, true, false);
            AppendOperation(template, sw, "deployments/{id}/clusters/{name}/routing", "DeploymentRouting", "Routing part of deployment", "Deployment", "#/definitions/Routing", deploymentAdditionalParameters, deploymentAdditionalParameters, deploymentAdditionalParameters, deploymentAdditionalParameters, true, false);

            sw.Write(debugOperations.ToString());
            sw.WriteLine();

            sw.Write("definitions:");
            sw.WriteLine();

            sw.Write(definitions.ToString());
            sw.WriteLine();

            sw.Close();

            RemoveEmptyLines("pre_output.txt", "vampopenapi.yaml");
        }

        public void AppendOperation(StringBuilder template, StreamWriter resultWriter, string operation, string capitalItem, string lowercaseItem, string tags, string definition, StringBuilder additionalGetParameters, StringBuilder additionalPostParameters, StringBuilder additionalPutParameters, StringBuilder additionalDeleteParameters, bool removeByNameOperations, bool removeDeleteOperations)
        {
            StringBuilder result = new StringBuilder(template.ToString());
            result = result.Replace("{operation}", operation);
            result = result.Replace("{capital_item}", capitalItem);
            result = result.Replace("{lowercase_item}", lowercaseItem);
            result = result.Replace("{tags}", tags);
            result = result.Replace("{definition}", definition);
            result = result.Replace("{additional_get_parameters}", additionalGetParameters.ToString());
            result = result.Replace("{additional_post_parameters}", additionalPostParameters.ToString());
            result = result.Replace("{additional_put_parameters}", additionalPutParameters.ToString());
            result = result.Replace("{additional_delete_parameters}", additionalDeleteParameters.ToString());
            if (removeByNameOperations)
            {
                Match match = Regex.Match(result.ToString(), "({byname_operations}[\\w\\W]*?{/byname_operations})");
                if (match.Groups.Count >= 1)
                    result = result.Replace(match.Groups[0].ToString(), "");
            }
            else
            {
                result = result.Replace("{byname_operations}", "");
                result = result.Replace("{/byname_operations}", "");

            }

            if (removeDeleteOperations)
            {
                Match match = Regex.Match(result.ToString(), "({delete_operation}[\\w\\W]*?{/delete_operation})");
                if (match.Groups.Count >= 1)
                    result = result.Replace(match.Groups[0].ToString(), "");
            }
            else
            {
                result = result.Replace("{delete_operation}", "");
                result = result.Replace("{/delete_operation}", "");

            }


            // ({byname_operations}[\w\W]*?{/byname_operations})

            resultWriter.Write(result);
            resultWriter.WriteLine();
        }


    }
}
