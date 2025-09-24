# Contributing to SynQcore

First off, thank you for considering contributing to SynQcore! 🎉

**SynQcore** is created and maintained by **[André César Vieira](https://github.com/andrecesarvieira)**, and we welcome contributions from the community to make this corporate social network platform even better.

## 👨‍💻 About the Project Creator

**André César Vieira** is an enterprise software architect with extensive experience in:
- .NET ecosystem and Clean Architecture
- PostgreSQL optimization and database design
- Corporate application development
- Performance engineering and scalability

## 🤝 How to Contribute

### 1. 🐛 Reporting Bugs

Before creating bug reports, please check the [existing issues](https://github.com/andrecesarvieira/synqcore/issues) to avoid duplicates.

When creating a bug report, include:
- Clear description of the problem
- Steps to reproduce the issue
- Expected vs actual behavior
- Environment details (OS, .NET version, etc.)
- Any relevant logs or screenshots

### 2. 💡 Suggesting Features

We welcome feature suggestions! Please:
- Check existing feature requests first
- Provide clear use case and business value
- Consider how it fits with corporate social network goals
- Include mockups or examples if helpful

### 3. 🔧 Code Contributions

#### Getting Started

1. **Fork** the repository
2. **Clone** your fork locally
3. **Create** a feature branch from `master`
4. **Setup** the development environment:
   ```bash
   # Start infrastructure
   ./scripts/start-dev.sh
   
   # Apply migrations
   dotnet ef database update -p src/SynQcore.Infrastructure -s src/SynQcore.Api
   
   # Run tests
   dotnet test
   ```

#### Development Guidelines

**Architecture Principles:**
- Follow **Clean Architecture** patterns
- Maintain **separation of concerns**
- Use **CQRS** pattern for complex operations
- Implement **proper error handling**
- Write **comprehensive tests**

**Code Standards:**
- Use **C# 12** language features appropriately
- Follow **Microsoft C# coding conventions**
- Add **XML documentation** for public APIs
- Maintain **consistent formatting** (EditorConfig)
- Keep **performance** in mind (use LoggerMessage, etc.)

**Database Guidelines:**
- Use **Entity Framework Core** migrations
- Follow **PostgreSQL best practices**
- Include **proper indexes** for performance
- Write **efficient queries**

#### Pull Request Process

1. **Update** documentation if needed
2. **Add tests** for new functionality
3. **Ensure** all tests pass
4. **Follow** commit message conventions:
   ```
   feat: add employee department transfer endpoint
   fix: resolve rate limiting bypass issue
   docs: update API documentation for authentication
   ```
5. **Create** pull request with:
   - Clear description of changes
   - Reference to related issues
   - Screenshots if UI changes
   - Performance impact notes if relevant

## 🧪 Testing Guidelines

### Running Tests
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/SynQcore.UnitTests
dotnet test tests/SynQcore.IntegrationTests

# With coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Writing Tests
- **Unit tests** for business logic
- **Integration tests** for API endpoints
- **Performance tests** for critical paths
- **Mock external dependencies** appropriately

## 📝 Documentation

### API Documentation
- Keep **Swagger/OpenAPI** definitions updated
- Include **request/response examples**
- Document **error scenarios**
- Explain **rate limiting** and **authentication**

### Code Documentation
- **XML comments** for public APIs
- **README updates** for new features
- **Architecture decision records** for significant changes

## 🎯 Areas Looking for Contributions

### High Priority
- **Authentication system** (JWT, roles, permissions)
- **Real-time features** (SignalR implementation)
- **File upload/media** handling
- **Advanced search** capabilities
- **Performance optimizations**

### Documentation & Examples
- **Tutorial content** for common scenarios
- **Architecture guides** and best practices
- **Deployment guides** for different environments
- **API usage examples** in different languages

### Testing & Quality
- **Increase test coverage** (target: >80%)
- **Performance benchmarks** and monitoring
- **Security testing** and hardening
- **Accessibility** improvements

## 🏆 Recognition

Contributors will be:
- **Listed** in CONTRIBUTORS.md
- **Mentioned** in release notes
- **Recognized** in project documentation
- **Invited** to join the core contributor team (for significant contributions)

## 📞 Getting Help

- **GitHub Issues** - For bugs and feature requests
- **GitHub Discussions** - For questions and general discussion
- **Email André** - [andrecesarvieira@hotmail.com](mailto:andrecesarvieira@hotmail.com) for direct communication

## 📋 Code of Conduct

### Our Commitment
We are committed to providing a welcoming and inspiring community for everyone.

### Expected Behavior
- **Be respectful** and inclusive
- **Welcome newcomers** and help them get started
- **Focus on constructive feedback**
- **Acknowledge contributions** from others
- **Prioritize project goals** over personal preferences

### Unacceptable Behavior
- Harassment, discrimination, or toxic behavior
- Spam, self-promotion unrelated to the project
- Publishing others' private information
- Any conduct inappropriate in a professional setting

## 🎉 Thank You!

Every contribution matters, from fixing typos to implementing major features. Thank you for helping make SynQcore the best open-source corporate social network platform!

---

**"Building the future of corporate collaboration, together."**  
*- André César Vieira & the SynQcore Community*