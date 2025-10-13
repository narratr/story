# Story Framework Examples

This project demonstrates the comprehensive usage of the Story framework with various scenarios including logging, data management, sub-stories, error handling, and async operations.

## Features Demonstrated

### Basic Scenarios
- **Logging Example**: Shows different log levels (Debug, Info, Warn, Error, Critical)
- **Data Example**: Demonstrates storing and retrieving data in stories
- **Performance Example**: Shows how to measure and log performance metrics
- **Workflow Example**: Demonstrates a multi-step workflow with data tracking

### Sub-Story Scenarios
- **Simple Sub-Story**: Basic parent-child story relationships
- **Nested Sub-Story**: Multi-level story hierarchies
- **Parallel Sub-Stories**: Multiple sub-stories running concurrently
- **Complex Hierarchy**: Tree-like story structures with multiple branches
- **Data Flow**: How data flows between parent and child stories

### Error Scenarios
- **Exception Handling**: Using the new `ReportFailed(Exception)` method
- **Error Messages**: Using the new `ReportFailed(string)` method
- **Sub-Story Errors**: Error handling in nested stories
- **Recovery**: Retry mechanisms and error recovery patterns
- **Error Data**: Storing error details in story data

### Async Scenarios
- **Basic Async**: Simple async operations with stories
- **Async Sub-Stories**: Async operations in sub-stories
- **Parallel Async**: Multiple concurrent async operations
- **Async Errors**: Error handling in async contexts
- **Async Data Flow**: Data management in async scenarios
- **Async Cancellation**: Cancellation token support
- **Async Nested**: Deep async story hierarchies

### Performance Scenarios
- **Performance Tests**: Comprehensive performance measurements and benchmarks
- **Memory Usage**: Memory consumption tracking and optimization
- **Execution Time**: Timing measurements for various operations
- **Throughput**: Operations per second measurements
- **Resource Usage**: CPU and memory usage patterns

## Configuration

### StoryLogStore Handler Configuration

The examples are configured to use the StoryLogStore handler, which batches and sends stories to a remote API. Update the `App.config` file with your actual configuration:

```xml
<appSettings>
    <!-- Replace with your actual Story Log Store API URL -->
    <add key="StoryLogStoreApiUrl" value="https://your-story-log-store-api.com" />
    
    <!-- Replace with your actual API key -->
    <add key="StoryLogStoreApiKey" value="your-api-key-here" />
</appSettings>
```

**Note**: If the StoryLogStore handler fails to configure (e.g., missing API URL or key), the examples will continue to run with the default handlers only.

## Running the Examples

### Running Regular Examples (Default)
By default, the application runs all regular scenarios including basic logging, sub-stories, error handling, and async operations.

#### Option 1: Using the Build Script
```cmd
cd Playground
run-examples.cmd
```

#### Option 2: Manual Build and Run
```cmd
# Build the solution
msbuild Story.sln /p:Configuration=Debug

# Run the examples
cd Playground
bin\Debug\Story.Examples.exe
```

#### Option 3: Using Visual Studio
1. Open `Story.sln` in Visual Studio
2. Set `Story.Examples` as the startup project
3. Press F5 to run

### Running Performance Tests Only
To run only the performance tests and benchmarks, use the `--runPerformanceTests` or `-p` command line argument:

#### Option 1: Using the Build Script with Performance Tests
```cmd
cd Playground
run-examples.cmd --runPerformanceTests
```

#### Option 2: Manual Build and Run Performance Tests
```cmd
# Build the solution
msbuild Story.sln /p:Configuration=Debug

# Run performance tests only
cd Playground
bin\Debug\Story.Examples.exe --runPerformanceTests
# or use the short form:
bin\Debug\Story.Examples.exe -p
```

#### Option 3: Using Visual Studio with Performance Tests
1. Open `Story.sln` in Visual Studio
2. Set `Story.Examples` as the startup project
3. Right-click on the project → Properties → Debug
4. In "Command line arguments", enter: `--runPerformanceTests`
5. Press F5 to run

## What You'll See

### Regular Examples
The examples will run through all scenarios and display:
- Story creation and lifecycle management
- Logging at different severity levels
- Data storage and retrieval
- Sub-story creation and nesting
- Error handling and reporting
- Async operation management
- Performance measurements
- Data flow between stories

### Performance Tests
When running with `--runPerformanceTests`, you'll see:
- Comprehensive performance benchmarks
- Memory usage statistics
- Execution time measurements
- Throughput metrics (operations per second)
- Resource consumption patterns
- Performance regression detection
- Optimization recommendations

## Key Story Framework Features Used

1. **Story Creation**: `Storytelling.StartNew()`
2. **Logging**: `Storytelling.Info()`, `Storytelling.Error()`, etc.
3. **Data Management**: `Storytelling.Data["key"] = value`
4. **Error Reporting**: `Storytelling.ReportFailed()` (new method)
5. **Sub-Stories**: Nested `using` statements with story creation
6. **Async Support**: `async`/`await` with story context preservation
7. **Story Lifecycle**: Automatic disposal and cleanup

## Project Structure

```
Playground/
├── Program.cs                 # Main entry point
├── Scenarios/
│   ├── BasicScenarios.cs     # Basic logging and data examples
│   ├── SubStoryScenarios.cs  # Sub-story and hierarchy examples
│   ├── ErrorScenarios.cs     # Error handling examples
│   ├── AsyncScenarios.cs     # Async operation examples
│   └── PerformanceScenarios.cs # Performance tests and benchmarks
├── Properties/
│   └── AssemblyInfo.cs       # Assembly information
├── App.config                # Application configuration
├── Story.Examples.csproj     # Project file
└── run-examples.cmd          # Build and run script
```

This comprehensive example suite demonstrates all the major features of the Story framework and serves as both documentation and a testing ground for the framework's capabilities.
