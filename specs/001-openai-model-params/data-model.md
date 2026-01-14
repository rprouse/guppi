# Data Model: OpenAI CLI Model Params

## Entities

### Model
- **modelName**: string (e.g., "gpt-3.5-turbo", "gpt-4")
- **supported**: bool (true if supported by skill)

### Parameter
- **temperature**: float (range: 0.0–2.0, default: 1.0)
- **maxTokens**: int (range: 1–4096, default: 1024)
- **otherParams**: dictionary (future extensibility)

## Relationships
- Each CLI invocation references one Model and zero or more Parameters.

## Validation Rules
- Model name must be in supported list
- Temperature must be within valid range
- Max tokens must be within valid range
- All parameters must be validated before request

## State Transitions
- Initial: CLI args parsed
- Valid: Model/params validated
- Error: Invalid/unsupported args → error message
- Ready: Valid args → OpenAI request
