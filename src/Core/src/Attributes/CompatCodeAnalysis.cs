#if !NET6_0_OR_GREATER
namespace System.Diagnostics.CodeAnalysis
{
	internal enum DynamicallyAccessedMemberTypes
	{
		All = -1,
		None = 0x0,
		PublicParameterlessConstructor = 0x1,
		PublicConstructors = 0x3,
		NonPublicConstructors = 0x4,
		PublicMethods = 0x8,
		NonPublicMethods = 0x10,
		PublicFields = 0x20,
		NonPublicFields = 0x40,
		PublicNestedTypes = 0x80,
		NonPublicNestedTypes = 0x100,
		PublicProperties = 0x200,
		NonPublicProperties = 0x400,
		PublicEvents = 0x800,
		NonPublicEvents = 0x1000,
		Interfaces = 0x2000
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, Inherited = false)]
	internal sealed class DynamicallyAccessedMembersAttribute : Attribute
	{
		public DynamicallyAccessedMembersAttribute(DynamicallyAccessedMemberTypes memberTypes)
		{
		}

		public DynamicallyAccessedMemberTypes MemberTypes => throw new NotImplementedException();
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method, Inherited = false)]
	internal sealed class RequiresUnreferencedCodeAttribute : Attribute
	{
		public string Message => throw new NotImplementedException();

		public string? Url
		{
			get => throw new NotImplementedException();
			set => throw new NotImplementedException();
		}

		public RequiresUnreferencedCodeAttribute(string message)
		{
		}
	}
}
#endif