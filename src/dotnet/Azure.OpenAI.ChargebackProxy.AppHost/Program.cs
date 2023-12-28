var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Azure_OpenAI_ChargebackProxy>("azure.openai.chargebackproxy");

builder.Build().Run();
