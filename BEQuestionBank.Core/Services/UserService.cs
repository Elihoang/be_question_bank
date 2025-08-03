using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEQuestionBank.Domain.Enums;
using BEQuestionBank.Shared.DTOs.user;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace BEQuestionBank.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IKhoaRepository _khoaRepository;

        public UserService(IUserRepository userRepository, IKhoaRepository khoaRepository)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            _userRepository = userRepository;
            _khoaRepository = khoaRepository;
        }

        public async Task<NguoiDung> GetByIdAsync(Guid maNguoiDung)
        {
            try
            {
                return await _userRepository.GetByIdAsync(maNguoiDung);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy thông tin người dùng theo ID: {MaNguoiDung}", maNguoiDung);
                throw;
            }
        }

        public async Task<NguoiDung> CreateAsync(NguoiDung user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "Dữ liệu người dùng không được để trống");
                }

                user.MaNguoiDung = Guid.NewGuid();
                user.MatKhau = BCrypt.Net.BCrypt.HashPassword(user.MatKhau);
                await _userRepository.AddAsync(user);
                return user;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi tạo người dùng: {User}", user);
                throw;
            }
        }

        public async Task<NguoiDung> UpdateAsync(Guid maNguoiDung, NguoiDung user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "Dữ liệu người dùng không được để trống");
                }

                var existingUser = await _userRepository.GetByIdAsync(maNguoiDung);
                if (existingUser == null)
                {
                    throw new Exception("Không tìm thấy người dùng");
                }

                existingUser.TenDangNhap = user.TenDangNhap;
                if (!string.IsNullOrEmpty(user.MatKhau))
                {
                    existingUser.MatKhau = BCrypt.Net.BCrypt.HashPassword(user.MatKhau);
                }
                existingUser.HoTen = user.HoTen;
                existingUser.Email = user.Email;
                existingUser.VaiTro = user.VaiTro;
                existingUser.BiKhoa = user.BiKhoa;
                existingUser.MaKhoa = user.MaKhoa;

                await _userRepository.UpdateAsync(existingUser);
                return existingUser;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi cập nhật người dùng với ID: {MaNguoiDung}", maNguoiDung);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid maNguoiDung)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(maNguoiDung);
                if (user == null)
                {
                    return false;
                }

                await _userRepository.DeleteAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi xóa người dùng với ID: {MaNguoiDung}", maNguoiDung);
                throw;
            }
        }

        public async Task<NguoiDung> GetByUsernameAsync(string tenDangNhap)
        {
            try
            {
                return await _userRepository.GetByUsernameAsync(tenDangNhap);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy thông tin người dùng theo TenDangNhap: {TenDangNhap}", tenDangNhap);
                throw;
            }
        }

        public async Task<IEnumerable<NguoiDung>> GetAllAsync()
        {
            try
            {
                return await _userRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi lấy tất cả thông tin người dùng");
                throw;
            }
        }

        public async Task<IEnumerable<NguoiDung>> FindAsync(System.Linq.Expressions.Expression<Func<NguoiDung, bool>> predicate)
        {
            try
            {
                return await _userRepository.FindAsync(predicate);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi tìm kiếm người dùng với điều kiện");
                throw;
            }
        }

        public async Task<NguoiDung> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<NguoiDung, bool>> predicate)
        {
            try
            {
                return await _userRepository.FirstOrDefaultAsync(predicate);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi tìm người dùng đầu tiên với điều kiện");
                throw;
            }
        }

        public async Task AddAsync(NguoiDung entity)
        {
            try
            {
                await _userRepository.AddAsync(entity);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi thêm người dùng: {User}", entity);
                throw;
            }
        }

        public async Task UpdateAsync(NguoiDung entity)
        {
            try
            {
                await _userRepository.UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi cập nhật người dùng: {User}", entity);
                throw;
            }
        }

        public async Task DeleteAsync(NguoiDung entity)
        {
            try
            {
                await _userRepository.DeleteAsync(entity);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi xóa người dùng: {User}", entity);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<NguoiDung, bool>> predicate)
        {
            try
            {
                return await _userRepository.ExistsAsync(predicate);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi kiểm tra sự tồn tại của người dùng với điều kiện");
                throw;
            }
        }

        public async Task<IEnumerable<NguoiDung>> GetUsersActiveAsync()
        {
            return await _userRepository.FindAsync(u => u.BiKhoa == false);
        }

        public async Task<IEnumerable<NguoiDung>> GetUsersLockedAsync()
        {
            return await _userRepository.FindAsync(u => u.BiKhoa == true);
        }

        public async Task<bool> LockUserAsync(Guid maNguoiDung)
        {
            var user = await _userRepository.GetByIdAsync(maNguoiDung);
            if (user == null) return false;
            user.BiKhoa = true;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> UnlockUserAsync(Guid maNguoiDung)
        {
            var user = await _userRepository.GetByIdAsync(maNguoiDung);
            if (user == null) return false;
            user.BiKhoa = false;
            await _userRepository.UpdateAsync(user);
            return true;
        }

      public async Task<(int SuccessCount, List<string> Errors)> ImportUsersFromExcelAsync(IFormFile file)
        {
            var errors = new List<string>();
            int successCount = 0;

            if (file == null || file.Length == 0)
            {
                errors.Add("File không được trống hoặc không hợp lệ.");
                return (successCount, errors);
            }

            if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                errors.Add("Định dạng file phải là .xlsx.");
                return (successCount, errors);
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null || worksheet.Dimension == null)
                        {
                            errors.Add("File Excel không chứa dữ liệu hợp lệ.");
                            return (successCount, errors);
                        }

                        int rowCount = worksheet.Dimension.Rows;
                        if (rowCount < 2)
                        {
                            errors.Add("File Excel không chứa dữ liệu người dùng.");
                            return (successCount, errors);
                        }

                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                var dto = new ImportUserDto
                                {
                                    TenDangNhap = worksheet.Cells[row, 1].Text?.Trim(),
                                    MatKhau = worksheet.Cells[row, 2].Text?.Trim(),
                                    HoTen = worksheet.Cells[row, 3].Text?.Trim(),
                                    Email = worksheet.Cells[row, 4].Text?.Trim(),
                                    VaiTro = int.TryParse(worksheet.Cells[row, 5].Text, out var vt) ? vt : 0,
                                    BiKhoa = bool.TryParse(worksheet.Cells[row, 6].Text, out var bk) && bk,
                                    TenKhoa = worksheet.Cells[row, 7].Text?.Trim()
                                };

                                // Validate required fields
                                if (string.IsNullOrWhiteSpace(dto.TenDangNhap))
                                {
                                    errors.Add($"Dòng {row}: Tên đăng nhập không được để trống.");
                                    continue;
                                }

                                if (string.IsNullOrWhiteSpace(dto.MatKhau))
                                {
                                    errors.Add($"Dòng {row}: Mật khẩu không được để trống.");
                                    continue;
                                }

                                if (string.IsNullOrWhiteSpace(dto.Email))
                                {
                                    errors.Add($"Dòng {row}: Email không được để trống.");
                                    continue;
                                }

                                // Validate email format
                                if (!IsValidEmail(dto.Email))
                                {
                                    errors.Add($"Dòng {row}: Email không đúng định dạng.");
                                    continue;
                                }

                                // Check for duplicates using repository
                                var isDuplicate = await _userRepository.ExistsAsync(u =>
                                    u.TenDangNhap == dto.TenDangNhap || u.Email == dto.Email);
                                
                                if (isDuplicate)
                                {
                                    errors.Add($"Dòng {row}: Tên đăng nhập hoặc Email đã tồn tại.");
                                    continue;
                                }

                                // Validate VaiTro
                                if (!Enum.IsDefined(typeof(VaiTroNguoiDung), dto.VaiTro))
                                {
                                    errors.Add($"Dòng {row}: Vai trò không hợp lệ (giá trị: {dto.VaiTro}). Các giá trị hợp lệ: {string.Join(", ", Enum.GetValues(typeof(VaiTroNguoiDung)).Cast<int>())}.");
                                    continue;
                                }

                                // Find MaKhoa from TenKhoa
                                Guid? maKhoa = null;
                                if (!string.IsNullOrWhiteSpace(dto.TenKhoa))
                                {
                                    var khoa = await _khoaRepository.GetByTenKhoaAsync(dto.TenKhoa);
                                    if (khoa == null)
                                    {
                                        errors.Add($"Dòng {row}: Tên khoa '{dto.TenKhoa}' không tồn tại.");
                                        continue;
                                    }
                                    maKhoa = khoa.MaKhoa;
                                }

                                // Create entity
                                var entity = new NguoiDung
                                {
                                    MaNguoiDung = Guid.NewGuid(),
                                    TenDangNhap = dto.TenDangNhap,
                                    MatKhau = BCrypt.Net.BCrypt.HashPassword(dto.MatKhau),
                                    HoTen = dto.HoTen,
                                    Email = dto.Email,
                                    VaiTro = (VaiTroNguoiDung)dto.VaiTro,
                                    BiKhoa = dto.BiKhoa,
                                    MaKhoa = maKhoa,
                                    NgayTao = DateTime.UtcNow,
                                    NgayCapNhap = DateTime.UtcNow,
                                    NgayDangNhapCuoi = null 
                                };

                                await _userRepository.AddAsync(entity);
                                successCount++;
                            }
                            catch (Exception exRow)
                            {
                                errors.Add($"Dòng {row}: Lỗi xử lý dữ liệu - {exRow.Message}");
                                Serilog.Log.Error(exRow, "Lỗi xử lý dòng {Row} khi nhập người dùng từ Excel.", row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "Lỗi khi nhập người dùng từ Excel");
                errors.Add($"Lỗi hệ thống: {ex.Message}");
            }

            return (successCount, errors);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}