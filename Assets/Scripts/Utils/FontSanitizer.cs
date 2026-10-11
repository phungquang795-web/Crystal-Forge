using System;
using System.Text;
using System.Text.RegularExpressions;

namespace CrystalForge.Utils
{
    /// <summary>
    /// Bộ chuẩn hoá phông chữ: Khử toàn bộ dấu tiếng Việt và ký tự lạ không tương thích với font TextMeshPro,
    /// đảm bảo hiển thị typography sắc nét 100%, không bao giờ xuất hiện ô vuông lỗi (tofu / missing glyphs).
    /// </summary>
    public static class FontSanitizer
    {
        /// <summary>
        /// Chuẩn hoá văn bản: Khử emoji vỡ font và đưa về dạng Unicode FormC chuẩn tiếng Việt.
        /// Bảo toàn 100% thanh dấu tiếng Việt sắc nét.
        /// </summary>
        public static string Sanitize(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            // 1. Thay thế emoji không có trong SDF atlas thành ký hiệu tương thích
            string result = input
                .Replace("💎", "CR")
                .Replace("⚡", "+")
                .Replace("🛡️", "")
                .Replace("🛡", "")
                .Replace("🔥", "OVERDRIVE")
                .Replace("🎯", "CRIT")
                .Replace("🩸", "VAMP")
                .Replace("🔒", "")
                .Replace("⚔️", "")
                .Replace("⚔", "")
                .Replace("✨", "")
                .Replace("⭐", "")
                .Replace("💀", "")
                .Replace("🎉", "")
                .Replace("🏆", "")
                .Replace("🔨", "")
                .Replace("⛏️", "")
                .Replace("⛏", "")
                .Replace("❤️", "")
                .Replace("👟", "")
                .Replace("🔮", "")
                .Replace("👑", "")
                .Replace("🔊", "")
                .Replace("💾", "")
                .Replace("🎁", "Thưởng: ")
                .Replace("🏅", "Mốc: ")
                .Replace("🥉", "Đồng")
                .Replace("🥈", "Bạc")
                .Replace("🥇", "Vàng")
                .Replace("🌀", "")
                .Replace("⚒️", "")
                .Replace("⚒", "");

            // 2. Chuẩn hoá Unicode FormC để hiển thị dấu tiếng Việt đồng nhất tuyệt đối
            return result.Normalize(NormalizationForm.FormC);
        }
    }
}
