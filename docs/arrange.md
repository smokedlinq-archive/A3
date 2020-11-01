# Arrange

The arrange step is where you setup all the dependencies needed for execute the act step against the SUT.

## Usage

The only parameter on the Arrange delegate is the [ArrangeBuilder](../src/A3/src/Arrange/ArrangeBuilder.cs). Below you will find the different features that can be used to setup the test.

If a return value is provided on the arrange delegate then the value will be used as the parameter to the [act](act.md) step.

```csharp
public void AddShouldNotThrow()
  => A3<Calculator>
    .Arrange<(int x, int y)>(_ => (1,1))
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y)))
    .Assert(result => result.Should().Be(2));
```

### Mocking dependencies

The `Mock<T>()` and `Mock<T>(Action<T>)` methods both return the mocked object. They can be used repeatedly to retrieve the same mocked object.

Mocks can be used to instantiate the SUT. For more information see the [act](act.md) step.

```csharp
public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange<Widget>(setup => 
    {
        var mock = setup.Mock<Widget>();
        mock.SetupGet(x => x.Value).Returns(1);
        return mock.Object;
    })
    .Act((sut, widget) => sut.Add(widget.Value, widget.Value))
    .Assert(result => result.Should().Be(2));
```

### Fixtures

AutoFixture provides support for all fixtures. The `IFixture` is provided on the [builder](../src/A3/src/Arrange/ArrangeBuilder.cs) via the `Fixture` property.

> Note: The AutoFixture.AutoMoq feature of AutoFixture is already added to the fixture so no need to add it.

Fixtures can be used to instantiate the SUT. For more information see the [act](act.md) step.

```csharp
public void AddShouldNotThrow()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => 
        setup.Fixture.Create<int>(), setup.Fixture.Create<int>())
    .Act((sut, parameters) => sut.Invoking(x => x.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().NotThrow());
```

#### Fixture customization

You can customize the fixture either during arrange or using the [ICustomizeFixture](../src/A3/src/AutoFixture/ICustomizeFixture.cs) interface or register a specific type using the [ICustomizeFixture&lt;T&gt;](../src/A3/src/AutoFixture/ICustomizeFixture.cs) interface.

Implementations of [ICustomizeFixture](../src/A3/src/AutoFixture/ICustomizeFixture.cs) are invoked first, followed by [ICustomizeFixture&lt;T&gt;](../src/A3/src/AutoFixture/ICustomizeFixture.cs).

> Note: There is no order to which interface are invoked for each type. This can cause unexpected fixture results if more than one type registers the same fixture type.

```csharp
public class CustomizeFixture : ICustomizeFixture
{
    public void Customize(IFixture fixture)
        => fixture.Register<IWidget>(() => WidgetFactory.Create(fixture.Create<int>()));
}

public class CustomizeWidgetFixture : ICustomizeFixture<IWidget>
{
    public IWidget Customize(IFixture fixture)
        => WidgetFactory.Create(fixture.Create<int>());   
}
```

##### Scoped fixture customization

If a fixture shouldn't be used globally by all tests, you can scoped fixtures by overriding the `ShouldCustomize` method on either interface.

```csharp
public static class FixtureScopes
{
    public const string? Default = null;
    public const string NonDefault = nameof(Default);
}

public class CustomizeFixtureWhenScoped : ICustomizeFixture
{
    public bool ShouldCustomize(string? scope)
        => scope == FixtureScopes.NonDefault;

    public void Customize(IFixture fixture)
        => fixture.Register<IWidget>(() => WidgetFactory.Create(fixture.Create<int>()));
}

public class CustomizeWidgetFixtureWhenScoped : ICustomizeFixture<IWidget>
{
    public bool ShouldCustomize(string? scope)
        => scope == FixtureScopes.NonDefault;

    public IWidget Customize(IFixture fixture)
        => WidgetFactory.Create(fixture.Create<int>());   
}
```

By default the scope is null for all tests. To override the scope for a particular test use the [A3ScopeAttribute](../src/A3/src/A3ScopeAttribute.cs) on the test method.

> Note: The scope can also be defined on the class and at the assembly level. The method scope will always be used over the class, and the class scope will always be used over the assembly scope.

```csharp
[assembly: A3Scope(FixtureScopes.Default)]

[A3Scope(FixtureScopes.NonDefault)]
public void WhenAddingTwoValuesShouldBeGreaterThanOrEqualZero()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => (1, 1))
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));
```

#### SUTs

By default the SUT is created using the first public constructor found via reflection. The parameters are resolved with any registered mocks then by the fixture.

##### Specify the SUT constructor

If you want to specify the constructor, you can use the `Constructor` method on either the [A3&lt;T&gt; class](../src/A3/src/A3.cs) before calling arrange or the [builder](../src/A3/src/Arrange/ArrangeBuilder.cs) during the arrange step.

```csharp
public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Constructor(type => type.GetConstructor(new[] { typeof(ILogger) }))
    .Arrange(setup => (1, 1))
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));

public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => 
    {
        setup.Constructor(type => type.GetConstructor(new[] { typeof(ILogger) }));
        return (1, 1);
    })
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));
```

##### Specify the SUT during arrangement

For more advance control on the creation of the SUT, you can use the [builder](../src/A3/src/Arrange/ArrangeBuilder.cs) `Sut` method during the arrange step.

> Note: The examples are equivalent, you should use one two use based on your scenario.

```csharp
public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => 
    {
        // The SUT is created before the act step runs but after arrange has completed
        //   this defers the instantiation until after arrange has completed
        setup.Sut(context => new Calculator(context.Fixture<ILogger>()));
        return (1, 1);
    })
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));

public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => 
    {
        // The SUT is created before the act step runs but after arrange has completed
        //   this defers the instantiation until after arrange has completed
        //   but doesn't use the context
        setup.Sut(() => new Calculator(context.Fixture<ILogger>()));
        return (1, 1);
    })
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));

public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => 
    {
        // The SUT is created at this point in the arrange method
        setup.Sut(new Calculator(context.Fixture<ILogger>()));
        return (1, 1);
    })
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));
```

#### Parameters

The SUT may need some input or parameters to execute, though this is optional. If your SUT does not need a parameter, you do not need to specify one.

You can specify the parameter either using the [builder](../src/A3/src/Arrange/ArrangeBuilder.cs) `Parameter` method during setup or by returning a value from the arrange step.

> Note: If you return a value from arrange then the parameter type must be specified on the `Arrange` method because the type cannot be infered, but provides automatic type detection for the `Act` method.

```csharp
public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange(setup => setup.Parameter((1, 1)))
    .Act((Calculator sut, (int x, int y)) => sut.Add(x, y))
    .Assert(result => result.Should().Be(2));

public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => ((1, 1)))
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));
```

## Using Xunit theories with AutoFixture

If you are using Xunit and want to use the fixture capabilities of A3 to customize AutoFixture add the A3.Xunit package. This will provide access to the [A3DataAttribute](../src/A3.Xunit/src/A3DataAttribute.cs) that uses AutoFixture to generate inputs.

All the same fixture customizations will be applied. The attribute also allows you to specify the A3 scope to use for fixture customization.

```csharp
[Theory]
[A3Data]
public void OnePlusOneShouldBeTwo(int x, int y)
  => A3<Calculator>
    .Arrange(setup => { })
    .Act(sut => sut.Add(x, y))
    .Assert(result => result.Should().Be(x+y));
```
