using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.Collaboration.Helpers;

/// <summary>
/// Helper para conversão de tipos de endorsement corporativo em informações de display
/// </summary>
public static class EndorsementTypeHelper
{
    private static readonly Dictionary<EndorsementType, (string DisplayName, string Icon, string Description)> _typeInfo = new()
    {
        [EndorsementType.Helpful] = ("Útil", "🔥", "Conteúdo resolve problema ou dúvida"),
        [EndorsementType.Insightful] = ("Perspicaz", "💡", "Traz nova perspectiva valiosa"),
        [EndorsementType.Accurate] = ("Preciso", "✅", "Informação correta e confiável"),
        [EndorsementType.Innovative] = ("Inovador", "🚀", "Ideia criativa ou solução nova"),
        [EndorsementType.Comprehensive] = ("Abrangente", "📚", "Cobre o tópico completamente"),
        [EndorsementType.WellResearched] = ("Bem Pesquisado", "🔍", "Fontes sólidas e dados confiáveis"),
        [EndorsementType.Actionable] = ("Aplicável", "⚡", "Pode ser implementado facilmente"),
        [EndorsementType.Strategic] = ("Estratégico", "🎯", "Alinhado com objetivos corporativos")
    };

    /// <summary>
    /// Obter nome de exibição do tipo de endorsement
    /// </summary>
    public static string GetDisplayName(EndorsementType type)
    {
        return _typeInfo.TryGetValue(type, out var info) ? info.DisplayName : type.ToString();
    }

    /// <summary>
    /// Obter ícone do tipo de endorsement
    /// </summary>
    public static string GetIcon(EndorsementType type)
    {
        return _typeInfo.TryGetValue(type, out var info) ? info.Icon : "👍";
    }

    /// <summary>
    /// Obter descrição do tipo de endorsement
    /// </summary>
    public static string GetDescription(EndorsementType type)
    {
        return _typeInfo.TryGetValue(type, out var info) ? info.Description : string.Empty;
    }

    /// <summary>
    /// Obter todas as informações do tipo de endorsement
    /// </summary>
    public static (string DisplayName, string Icon, string Description) GetTypeInfo(EndorsementType type)
    {
        return _typeInfo.TryGetValue(type, out var info) ? info : (type.ToString(), "👍", string.Empty);
    }

    /// <summary>
    /// Obter todos os tipos de endorsement disponíveis com informações
    /// </summary>
    public static Dictionary<EndorsementType, (string DisplayName, string Icon, string Description)> GetAllTypes()
    {
        return new Dictionary<EndorsementType, (string DisplayName, string Icon, string Description)>(_typeInfo);
    }

    /// <summary>
    /// Calcular score de engajamento baseado nos tipos de endorsement
    /// </summary>
    public static double CalculateEngagementScore(Dictionary<EndorsementType, int> endorsementCounts)
    {
        // Pesos corporativos para cada tipo de endorsement
        var weights = new Dictionary<EndorsementType, double>
        {
            [EndorsementType.Helpful] = 1.0,
            [EndorsementType.Insightful] = 1.5,
            [EndorsementType.Accurate] = 1.2,
            [EndorsementType.Innovative] = 2.0,
            [EndorsementType.Comprehensive] = 1.3,
            [EndorsementType.WellResearched] = 1.4,
            [EndorsementType.Actionable] = 1.6,
            [EndorsementType.Strategic] = 2.5
        };

        double totalScore = 0;
        foreach (var (type, count) in endorsementCounts)
        {
            if (weights.TryGetValue(type, out var weight))
            {
                totalScore += count * weight;
            }
            else
            {
                totalScore += count; // Peso padrão 1.0
            }
        }

        return Math.Round(totalScore, 2);
    }

    /// <summary>
    /// Obter tipos de endorsement mais valiosos (maior peso)
    /// </summary>
    public static List<EndorsementType> GetHighValueTypes()
    {
        return new List<EndorsementType>
        {
            EndorsementType.Strategic,
            EndorsementType.Innovative,
            EndorsementType.Actionable,
            EndorsementType.Insightful
        };
    }

    /// <summary>
    /// Verificar se é um tipo de endorsement de alta qualidade
    /// </summary>
    public static bool IsHighQualityEndorsement(EndorsementType type)
    {
        var highQualityTypes = new[]
        {
            EndorsementType.Strategic,
            EndorsementType.Innovative,
            EndorsementType.WellResearched,
            EndorsementType.Comprehensive,
            EndorsementType.Insightful
        };

        return highQualityTypes.Contains(type);
    }
}