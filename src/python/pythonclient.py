import os
import textwrap
from dotenv import load_dotenv
from openai import AzureOpenAI

load_dotenv()

openai_client = AzureOpenAI(
    api_key = os.getenv("AZURE_OPENAI_API_KEY"),
    api_version = os.getenv("OPENAI_API_VERSION"),
    azure_endpoint = os.getenv("AZURE_OPENAI_API_BASE_URL")
)

chat_deployment_name = "gpt-4"

question = "When was Microsoft founded?"

response = openai_client.chat.completions.create(
    model = chat_deployment_name,
    messages=[
        {
            "role": "user",
            "content": question
        }
    ]
)

lines = textwrap.wrap(response.choices[0].message.content, width=80)
for line in lines:
    print(line)