using BEQuestionBank.Domain.Interfaces.Repo;
using BEQuestionBank.Domain.Interfaces.Service;
using BEQuestionBank.Domain.Models;
using BEQuestionBank.Shared.DTOs.DeThi;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BEQuestionBank.Domain.Enums;
using BEQuestionBank.Shared.DTOs;
using BEQuestionBank.Shared.DTOs.CauHoi;
using BEQuestionBank.Shared.DTOs.CauTraLoi;
using BEQuestionBank.Shared.DTOs.ChiTietDeThi;
using BEQuestionBank.Shared.Helpers;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OfficeOpenXml;
using Serilog;

namespace BEQuestionBank.Core.Services
{
    public class DeThiService : IDeThiService
    {
        private readonly IDeThiRepository _deThiRepository;
        private readonly IYeuCauRutTrichRepository _yeuCauRutTrichRepository;
        private readonly ICauHoiRepository _cauHoiRepository;
        private readonly ICauTraLoiRepository _cauTraLoiRepository;
        private readonly IPhanRepository _phanRepository;

        public DeThiService(IDeThiRepository deThiRepository, IYeuCauRutTrichRepository yeuCauRutTrichRepository,
            ICauHoiRepository cauHoiRepository, ICauTraLoiRepository cauTraLoiRepository , IPhanRepository phanRepository)
        {
            _deThiRepository = deThiRepository;
            _yeuCauRutTrichRepository = yeuCauRutTrichRepository;
            _cauHoiRepository = cauHoiRepository;
            _cauTraLoiRepository = cauTraLoiRepository;
            _phanRepository = phanRepository;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public Task<DeThi> GetByIdAsync(Guid id)
        {
            return _deThiRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<DeThi>> GetAllAsync()
        {
            return _deThiRepository.GetAllAsync();
        }

        public Task<IEnumerable<DeThi>> FindAsync(Expression<Func<DeThi, bool>> predicate)
        {
            return _deThiRepository.FindAsync(predicate);
        }

        public Task<DeThi> FirstOrDefaultAsync(Expression<Func<DeThi, bool>> predicate)
        {
            return _deThiRepository.FirstOrDefaultAsync(predicate);
        }

        public Task AddAsync(DeThi entity)
        {
            return _deThiRepository.AddAsync(entity);
        }

        public Task UpdateAsync(DeThi entity)
        {
            return _deThiRepository.UpdateAsync(entity);
        }

        public Task DeleteAsync(DeThi entity)
        {
            return _deThiRepository.DeleteAsync(entity);
        }

        public Task<bool> ExistsAsync(Expression<Func<DeThi, bool>> predicate)
        {
            return _deThiRepository.ExistsAsync(predicate);
        }

        public Task<DeThiDto> GetByIdWithChiTietAsync(Guid id)
        {
            return _deThiRepository.GetByIdWithChiTietAsync(id);
        }

        public Task<IEnumerable<DeThiDto>> GetAllWithChiTietAsync()
        {
            return _deThiRepository.GetAllWithChiTietAsync();
        }

        public Task<DeThiDto> AddWithChiTietAsync(DeThiDto deThiDto)
        {
            return _deThiRepository.AddWithChiTietAsync(deThiDto);
        }

        public Task<DeThiDto> UpdateWithChiTietAsync(DeThiDto deThiDto)
        {
            return _deThiRepository.UpdateWithChiTietAsync(deThiDto);
        }

        public async Task<IEnumerable<DeThi>> GetByMaMonHocAsync(Guid maMonHoc)
        {
            return await _deThiRepository.GetByMaMonHocAsync(maMonHoc);
        }

        public async Task<IEnumerable<DeThiDto>> GetApprovedDeThisAsync()
        {
            return await _deThiRepository.GetApprovedDeThisAsync();
        }

        public Task<DeThiDto> ImportMaTranFromExcelAsync(Guid maYeuCau, IFormFile excelFile)
        {
            throw new NotImplementedException();
        }
        
        public async Task<DeThiDto> ChangerStatusAsync(Guid id, bool DaDuyet)
        {
            var deThi = await _deThiRepository.GetByIdAsync(id);
            if (deThi == null)
                throw new Exception($"Không tìm thấy đề thi với mã {id}.");

            deThi.DaDuyet = DaDuyet;
            deThi.NgayCapNhap = DateTime.UtcNow;

            await _deThiRepository.UpdateAsync(deThi);

            return new DeThiDto
            {
                MaDeThi = deThi.MaDeThi,
                MaMonHoc = deThi.MaMonHoc,
                TenDeThi = deThi.TenDeThi,
                DaDuyet = deThi.DaDuyet,
                SoCauHoi = deThi.SoCauHoi,
                NgayTao = deThi.NgayTao,
                NgayCapNhap = deThi.NgayCapNhap
            };
        }

        public Task<MemoryStream> ExportWordTemplateAsync(Guid maDeThi)
        {
            throw new NotImplementedException();
        }

        public async Task<MemoryStream> ExportWordTemplateAsync(Guid maDeThi,
            ExamTemplateParametersDto parameters = null)
        {
            var deThiDto = await _deThiRepository.GetDeThiWithChiTietAndCauTraLoiAsync(maDeThi);
            if (deThiDto == null)
            {
                return null;
            }

            return await HtmlConverter.ExportWordTemplateAsync(deThiDto, parameters);
        }

        public async Task<IEnumerable<CauTraLoiDto>> GetCauTraLoiByDeThiAsync(Guid maDeThi)
        {
            var deThiDto = await _deThiRepository.GetByIdWithChiTietAsync(maDeThi);
            if (deThiDto == null || deThiDto.ChiTietDeThis == null)
            {
                return Enumerable.Empty<CauTraLoiDto>();
            }

            var maCauHoiList = deThiDto.ChiTietDeThis.Select(ct => ct.MaCauHoi).Distinct().ToList();
            var cauTraLoiList = await _cauTraLoiRepository.FindAsync(ct => maCauHoiList.Contains(ct.MaCauHoi));

            var cauTraLoiDtos = cauTraLoiList.Select(ct => new CauTraLoiDto
            {
                MaCauTraLoi = ct.MaCauTraLoi,
                MaCauHoi = ct.MaCauHoi,
                NoiDung = ct.NoiDung,
                ThuTu = ct.ThuTu,
                LaDapAn = ct.LaDapAn,
                HoanVi = ct.HoanVi
            }).ToList();

            return cauTraLoiDtos;
        }

        public async Task<DeThiWithChiTietAndCauTraLoiDto> GetDeThiWithChiTietAndCauTraLoiAsync(Guid maDeThi)
        {
            var deThi = await _deThiRepository.GetDeThiWithChiTietAndCauTraLoiAsync(maDeThi);

            if (deThi == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy đề thi với MaDeThi: {maDeThi}");
            }

            var cauHoiDict = deThi.ChiTietDeThis
                .ToDictionary(ct => ct.MaCauHoi, ct => ct.CauHoi);

            var chiTietChaList = new List<ChiTietDeThiDto>();

            foreach (var ct in deThi.ChiTietDeThis)
            {
                var cauHoi = ct.CauHoi;

                if (!cauHoi.MaCauHoiCha.HasValue)
                {
                    chiTietChaList.Add(ct);

                    if (cauHoi.CauHoiCons == null)
                        cauHoi.CauHoiCons = new List<CauHoiDto>();
                }
                else
                {
                    if (cauHoiDict.TryGetValue(cauHoi.MaCauHoiCha.Value, out var cauHoiCha))
                    {
                        if (cauHoiCha.CauHoiCons == null)
                            cauHoiCha.CauHoiCons = new List<CauHoiDto>();

                        cauHoiCha.CauHoiCons.Add(cauHoi);
                    }
                    else
                    {
                        chiTietChaList.Add(ct);
                    }
                }
            }

            deThi.ChiTietDeThis = chiTietChaList;

            return deThi;
        }
       public async Task<DeThiDto> RutTrichDeThiFromYeuCauAsync(Guid maYeuCau)
        {
            var yeuCau = await _yeuCauRutTrichRepository.GetByIdAsync(maYeuCau);
            if (yeuCau == null)
                throw new Exception($"Không tìm thấy yêu cầu {maYeuCau}");

            if (string.IsNullOrWhiteSpace(yeuCau.MaTran))
                throw new Exception("MaTran không được để trống.");

            var checkResult = await CheckExtractionAsync(yeuCau.MaMonHoc, yeuCau.MaTran);
            if (!checkResult.CanExtract)
                throw new Exception(checkResult.Message);

            var request = ParseMaTranRequest(yeuCau.MaTran);
            var units = await _cauHoiRepository.GetQuestionUnitsByMonHocAsync(yeuCau.MaMonHoc);
            var selectedUnits = SelectExactSubsetUnits(units, request);

            var deThiDto = new DeThiDto
            {
                MaDeThi = Guid.NewGuid(),
                MaMonHoc = yeuCau.MaMonHoc,
                TenDeThi = $"Đề thi từ yêu cầu {maYeuCau}",
                DaDuyet = false,
                SoCauHoi = checkResult.TotalSuccess,
                NgayTao = DateTime.UtcNow,
                NgayCapNhap = DateTime.UtcNow,
                ChiTietDeThis = new List<ChiTietDeThiDto>()
            };

            int thuTu = 1;
            var usedCauHoiIds = new HashSet<Guid>();
            foreach (var unit in selectedUnits)
            {
                foreach (var q in unit.Questions)
                {
                    if (!usedCauHoiIds.Add(q.MaCauHoi)) continue;

                    deThiDto.ChiTietDeThis.Add(new ChiTietDeThiDto
                    {
                        MaDeThi = deThiDto.MaDeThi,
                        MaPhan = q.MaPhan,
                        MaCauHoi = q.MaCauHoi,
                        ThuTu = thuTu++
                    });
                }
            }

            if (deThiDto.ChiTietDeThis.Count == 0)
                throw new Exception("Không có câu hỏi nào được rút trích.");

            var saved = await _deThiRepository.AddWithChiTietAsync(deThiDto);
            return saved;
        }
        
        
        /// <summary>
        /// Hàm tách: Parse MaTran từ string JSON.
        /// </summary>
        private RutTrichRequest ParseMaTranRequest(string maTran)
        {
            try
            {
                var raw = maTran;
                if (raw.StartsWith("\"") && raw.EndsWith("\""))
                {
                    raw = JsonConvert.DeserializeObject<string>(raw);
                }

                var request = JsonConvert.DeserializeObject<RutTrichRequest>(raw);
                if (request == null)
                    throw new Exception("Deserialize MaTran trả về null.");

                return request;
            }
            catch (JsonException ex)
            {
                Serilog.Log.Error(ex, "Lỗi parse MaTran: {MaTran}", maTran);
                throw new Exception($"MaTran không phải JSON hợp lệ. Chi tiết: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Hàm tách: Chọn subset units phù hợp với yêu cầu CLO.
        /// </summary>
       private List<QuestionUnit> SelectExactSubsetUnits(List<QuestionUnit> units, RutTrichRequest request)
        {
            var cloDict = request.Clos.ToDictionary(c => c.Clo, c => c.Num);

            List<QuestionUnit> selectedUnits;

            if (request.CloPerPart)
            {
                selectedUnits = new List<QuestionUnit>();
                foreach (var part in request.Parts)
                {
                    var partUnits = units.Where(u => u.MaPhan == part.MaPhan).ToList();
                    var partCloDict = part.Clos.ToDictionary(c => c.Clo, c => c.Num);

                    Serilog.Log.Information("Đang xử lý Part={MaPhan}, Yêu cầu CLO={Clos}",
                        part.MaPhan,
                        string.Join(", ", partCloDict.Select(kvp => $"CLO{kvp.Key}={kvp.Value}")));

                    var partSelected = FindExactSubset(partUnits, partCloDict);
                    if (partSelected == null)
                    {
                        var unitInfo = string.Join(" | ", partUnits.Select(u =>
                            $"Unit:{u.Id}, CLOs:[{string.Join(",", u.CloCounts.Select(c => $"{c.Key}:{c.Value}"))}], Total={u.TotalQuestions}"
                        ));

                        Serilog.Log.Warning("Không tìm thấy subset cho Part={MaPhan}. Units hiện có: {UnitInfo}", part.MaPhan, unitInfo);

                        throw new Exception($"Không tìm thấy tập hợp câu hỏi thỏa mãn cho phần {part.MaPhan}.");
                    }

                    selectedUnits.AddRange(partSelected);
                }
            }
            else
            {
                selectedUnits = FindExactSubset(units, cloDict);

                if (selectedUnits == null)
                {
                    var cloInfo = string.Join(", ", cloDict.Select(kvp => $"CLO{kvp.Key}={kvp.Value}"));
                    var unitInfo = string.Join(" | ", units.Select(u =>
                        $"Unit:{u.Id}, MaPhan:{u.MaPhan}, CLOs:[{string.Join(",", u.CloCounts.Select(c => $"{c.Key}:{c.Value}"))}], Total={u.TotalQuestions}"
                    ));

                    Serilog.Log.Warning("Không tìm thấy tập hợp câu hỏi. Yêu cầu CLO: {CloInfo}. Units hiện có: {UnitInfo}", cloInfo, unitInfo);

                    throw new Exception("Không tìm thấy tập hợp câu hỏi thỏa mãn yêu cầu CLO exact.");
                }
            }

            return selectedUnits;
        }
        /// <summary>
        /// Hàm tách: Tạo và lưu đề thi từ selectedUnits.
        /// </summary>
        private async Task<DeThiDto> CreateAndSaveDeThiFromSelectedUnits(YeuCauRutTrich yeuCau, RutTrichRequest request, List<QuestionUnit> selectedUnits)
        {
            // Kiểm tra số câu hỏi
            int totalSelected = selectedUnits.Sum(u => u.TotalQuestions);
            if (totalSelected != request.TotalQuestions)
            {
                Serilog.Log.Warning(
                    "Số lượng không khớp. Yêu cầu={Req}, Thực tế={Real}, Units={Units}",
                    request.TotalQuestions,
                    totalSelected,
                    string.Join(" | ", selectedUnits.Select(u =>
                        $"Unit:{u.Id}, CLOs:[{string.Join(",", u.CloCounts.Select(c => $"{c.Key}:{c.Value}"))}], Total={u.TotalQuestions}"
                    ))
                );

                throw new Exception("Tổng số câu hỏi rút trích không khớp với yêu cầu.");
            }

            // Tạo đề thi DTO
            var deThiDto = new DeThiDto
            {
                MaDeThi = Guid.NewGuid(),
                MaMonHoc = yeuCau.MaMonHoc,
                TenDeThi = $"Đề thi từ yêu cầu {yeuCau.MaYeuCau}",
                DaDuyet = false,
                SoCauHoi = totalSelected,
                NgayTao = DateTime.UtcNow,
                NgayCapNhap = DateTime.UtcNow,
                ChiTietDeThis = new List<ChiTietDeThiDto>()
            };

            int thuTu = 1;
            var usedCauHoiIds = new HashSet<Guid>();
            foreach (var unit in selectedUnits)
            {
                foreach (var q in unit.Questions)
                {
                    if (!usedCauHoiIds.Add(q.MaCauHoi))
                        continue;

                    deThiDto.ChiTietDeThis.Add(new ChiTietDeThiDto
                    {
                        MaDeThi = deThiDto.MaDeThi,
                        MaPhan = q.MaPhan,
                        MaCauHoi = q.MaCauHoi,
                        ThuTu = thuTu++
                    });
                }
            }

            if (deThiDto.ChiTietDeThis.Count == 0)
            {
                Serilog.Log.Warning("Không có câu hỏi nào được rút trích từ các Units đã chọn.");
                throw new Exception("Không có câu hỏi nào được rút trích.");
            }

            var saved = await _deThiRepository.AddWithChiTietAsync(deThiDto);
            return saved;
        }

        private List<QuestionUnit> FindExactSubset(
            List<QuestionUnit> units,
            Dictionary<int, int> req,
            int index = 0,
            List<QuestionUnit> selected = null,
            Dictionary<int, int> current = null)
        {
            selected ??= new List<QuestionUnit>();
            current ??= req.Keys.ToDictionary(k => k, _ => 0);

            if (index == units.Count)
            {
                if (req.All(kv => current.GetValueOrDefault(kv.Key) == kv.Value))
                {
                    return new List<QuestionUnit>(selected);
                }
                return null;
            }

            var skip = FindExactSubset(units, req, index + 1, selected, new Dictionary<int, int>(current));
            if (skip != null) return skip;

            var unit = units[index];
            var newCurrent = new Dictionary<int, int>(current);

            bool extra = false;
            foreach (var kv in unit.CloCounts)
            {
                var cloInt = kv.Key; 
                if (!req.ContainsKey(cloInt) && kv.Value > 0)
                {
                    extra = true;
                    break;
                }
                newCurrent[cloInt] = newCurrent.GetValueOrDefault(cloInt) + kv.Value;
                if (newCurrent[cloInt] > req.GetValueOrDefault(cloInt))
                    return null;
            }

            if (extra) return null;

            selected.Add(unit);
            var take = FindExactSubset(units, req, index + 1, selected, newCurrent);
            if (take != null) return take;
            selected.RemoveAt(selected.Count - 1);

            return null;
        }
        public async Task<ExtractionCheckResultDto> CheckExtractionAsync(Guid maMonHoc, string maTran)
        {
            if (string.IsNullOrWhiteSpace(maTran))
                throw new Exception("MaTran không được để trống.");

            var request = ParseMaTranRequest(maTran);

            var units = await _cauHoiRepository.GetQuestionUnitsByMonHocAsync(maMonHoc);
            Serilog.Log.Information("Tổng Units load được cho check: {Count}", units.Count);

            var checkResult = new ExtractionCheckResultDto
            {
                Message = "OK",
                CanExtract = true,
                CloExtracted = new Dictionary<int, int>()
            };

            List<QuestionUnit> selectedUnits = null;
            try
            {
                selectedUnits = SelectExactSubsetUnits(units, request);
            }
            catch (Exception ex)
            {
                checkResult.Message = ex.Message;
                checkResult.CanExtract = false;
                return checkResult;
            }

            // Tính toán số lượng câu hỏi đơn và nhóm từ selectedUnits
            checkResult.SingleQuestions = selectedUnits?.Count(u => !u.IsGroup) ?? 0;
            checkResult.GroupQuestions = selectedUnits?.Count(u => u.IsGroup) ?? 0;

            int totalSelected = selectedUnits?.Sum(u => u.TotalQuestions) ?? 0;
            checkResult.TotalSuccess = totalSelected;
            checkResult.TotalFailure = request.TotalQuestions - totalSelected;

            foreach (var unit in selectedUnits ?? new List<QuestionUnit>())
            {
                foreach (var kv in unit.CloCounts)
                {
                    if (!checkResult.CloExtracted.ContainsKey(kv.Key))
                        checkResult.CloExtracted[kv.Key] = 0;
                    checkResult.CloExtracted[kv.Key] += kv.Value;
                }
            }

            if (totalSelected != request.TotalQuestions)
            {
                checkResult.Message = "Tổng số câu hỏi lấy được không khớp yêu cầu.";
                checkResult.CanExtract = false;
            }

            return checkResult;
        }

        public async Task<MonHocStatsDto> GetMonHocStatsAsync(Guid maMonHoc)
        {
            var phans = await _phanRepository.GetByMaMonHocAsync(maMonHoc);
            var units = await _cauHoiRepository.GetQuestionUnitsByMonHocAsync(maMonHoc);

            var stats = new MonHocStatsDto
            {
                CloPerPart = phans.Any(),
                TotalQuestions = units.Sum(u => u.TotalQuestions),
                SingleQuestions = units.Count(u => !u.IsGroup),
                GroupQuestions = units.Count(u => u.IsGroup),
            };

            // ✅ Sử dụng key dạng số "1".."5"
            stats.CloTotals = new Dictionary<string, int>
            {
                { "1", 0 },
                { "2", 0 },
                { "3", 0 },
                { "4", 0 },
                { "5", 0 }
            };

            foreach (var unit in units)
            {
                foreach (var kv in unit.CloCounts) // kv.Key = int CLO, kv.Value = số câu
                {
                    var key = kv.Key.ToString(); // "1".."5"
                    if (stats.CloTotals.ContainsKey(key))
                        stats.CloTotals[key] += kv.Value;
                }
            }

            stats.Matrix = phans.Select(p =>
            {
                var phanUnits = units.Where(u => u.MaPhan == p.MaPhan).ToList();

                return new MatrixDto
                {
                    Chapter = p.TenPhan,
                    MaPhan = p.MaPhan,
                    CLO1 = phanUnits.Sum(u => u.CloCounts.GetValueOrDefault(1, 0)),
                    CLO2 = phanUnits.Sum(u => u.CloCounts.GetValueOrDefault(2, 0)),
                    CLO3 = phanUnits.Sum(u => u.CloCounts.GetValueOrDefault(3, 0)),
                    CLO4 = phanUnits.Sum(u => u.CloCounts.GetValueOrDefault(4, 0)),
                    CLO5 = phanUnits.Sum(u => u.CloCounts.GetValueOrDefault(5, 0)),
                    Total = phanUnits.Sum(u => u.TotalQuestions)
                };
            }).ToList();

            if (!stats.CloPerPart)
            {
                stats.Matrix.Add(new MatrixDto
                {
                    Chapter = "Default",
                    Total = stats.TotalQuestions
                });
            }

            return stats;
        }


        public Task<MonHocStatsDto> UpdateMatrixAsync(Guid maMonHoc, List<MatrixUpdateDto> updates)
        {
            throw new NotImplementedException();
        }
    }
}