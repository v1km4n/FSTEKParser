using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSTEKParser
{
    public class Threat
    {
        public static List<Threat> AllThreats { get; set; } = new List<Threat>();
        public static Dictionary<string, string> AllThreatsShort { get; set; } = new Dictionary<string, string>();
        public int ThreatId { get; set; }
        public string ThreatName { get; set; }
        public string ThreatDescription { get; set; }
        public string ThreatSource { get; set; }
        public string ThreatTarget { get; set; }
        public bool IsConfidentialityAffected { get; set; }
        public bool IsIntegrityAffected { get; set; }
        public bool IsAvailabilityAffected { get; set; }
        public Threat(int threatId, string threatName, string threatDescription, string threatSource, string threatTarget, bool isConfidentialityAffected, bool isIntegrityAffected, bool isAvailabilityAffected)
        {
            ThreatId = threatId;
            ThreatName = threatName;
            ThreatDescription = threatDescription;
            ThreatSource = threatSource;
            ThreatTarget = threatTarget;
            IsConfidentialityAffected = isConfidentialityAffected;
            IsIntegrityAffected = isIntegrityAffected;
            IsAvailabilityAffected = isAvailabilityAffected;
        }
        public override string ToString()
        {
            return $"ID: {ThreatId}\n" +
                $"Название: {ThreatName}\n" +
                $"Описание: {ThreatDescription}\n" +
                $"Источник: {ThreatSource}\n" +
                $"Объект: {ThreatTarget}\n" +
                $"Нарушение конфиденциальности: {(IsConfidentialityAffected ? "Да" : "Нет")}\n" +
                $"Нарушение целостности: {(IsIntegrityAffected ? "Да" : "Нет")}\n" +
                $"Нарушение доступности: {(IsAvailabilityAffected ? "Да" : "Нет")}";
        }
    }
}
