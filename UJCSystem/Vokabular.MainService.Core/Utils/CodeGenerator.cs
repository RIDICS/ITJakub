using Ridics.Core.Shared.Utils.CodeGenerators;

namespace Vokabular.MainService.Core.Utils
{
    public class CodeGenerator
    {
        public const int UserGroupNameLength = 8;

        private readonly IGenericCodeGenerator m_genericCodeGenerator;
        private readonly char[] m_allowedCharacters;
        
        public CodeGenerator()
        {
            m_genericCodeGenerator = new CryptographyCodeGenerator();
            m_allowedCharacters = new AllowedCharactersBuilder()
                .AddUpperAlphabet()
                .AddLowerAlphabet()
                .AddNumeric()
                .Build();
        }

        public string Generate(int length)
        {
            var result = m_genericCodeGenerator.GenerateCode(length, m_allowedCharacters);
            return result;
        }
    }
}
