using SynQcore.Domain.Entities.Communication;

namespace SynQcore.Application.Features.Collaboration.Helpers;

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

    public static string GetDisplayName(EndorsementType type)
    {
        return _typeInfo.TryGetValue(type, out var info) ? info.DisplayName : type.ToString();
    }

    public static string GetIcon(EndorsementType type)
    {
        return _typeInfo.TryGetValue(type, out var info) ? info.Icon : "👍";
    }

    public static string GetDescription(EndorsementType type)
    {
        return _typeInfo.TryGetValue(type, out var info) ? info.Description : string.Empty;
    }

    public static (string DisplayName, string Icon, string Description) GetTypeInfo(EndorsementType type)
    {
        return _typeInfo.TryGetValue(type, out var info) ? info : (type.ToString(), "👍", string.Empty);
    }

    public static Dictionary<EndorsementType, (string DisplayName, string Icon, string Description)> GetAllTypes()
    {
        return new Dictionary<EndorsementType, (string DisplayName, string Icon, string Description)>(_typeInfo);
    }

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