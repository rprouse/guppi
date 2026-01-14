# Quickstart: OpenAI CLI Model Params

## Prerequisites
- .NET 9.0+ installed
- API key configured securely

## Usage
```sh
# Specify model and parameters
Guppi.Console openai --model gpt-4 --temperature 0.7 --max-tokens 2048

# Show help
Guppi.Console openai --help
```

## Integration
- Register skill via DI in Guppi.Core
- Ensure Spectre.Console output for all CLI interactions
- All parameters validated before OpenAI request
- Test with xUnit and Moq
