using AutoMapper;
using Contracts;
using Entities.Exceptions.Line;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects.Line;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class LineService : ILineService
    {
        private readonly IRepositoryManager _repository;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public LineService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LineDto>> GetAllLinesAsync(bool trackChanges)
        {
            var lines = await _repository.Line.GetAllLinesAsync(trackChanges);
            var linesDto = _mapper.Map<IEnumerable<LineDto>>(lines);
            return linesDto;
        }

        public async Task<LineDto> GetLineAsync(int lineId, bool trackChanges)
        {
            var line = await GetLineAndCheckIfItExists(lineId, trackChanges);
            var lineDto = _mapper.Map<LineDto>(line);
            return lineDto;
        }

        public async Task<LineDto> GetLineByNumberAsync(int lineNumber, bool trackChanges)
        {
            var line = await _repository.Line.GetLineByNumberAsync(lineNumber, trackChanges);
            if (line is null)
                throw new LineNotFoundException(0); // ID không quan trọng vì dùng Number để tìm

            var lineDto = _mapper.Map<LineDto>(line);
            return lineDto;
        }

        public async Task<LineDto> CreateLineAsync(LineForCreationDto line)
        {
            if (line == null)
                throw new ArgumentNullException(nameof(line), "LineForCreationDto cannot be null.");

            var lineEntity = _mapper.Map<Line>(line);
            _repository.Line.CreateLine(lineEntity);
            await _repository.SaveAsync();

            var lineToReturn = _mapper.Map<LineDto>(lineEntity);
            return lineToReturn;
        }

        public async Task UpdateLineAsync(int lineId, LineForUpdateDto lineForUpdate, bool trackChanges)
        {
            var line = await GetLineAndCheckIfItExists(lineId, trackChanges);
            _mapper.Map(lineForUpdate, line);
            await _repository.SaveAsync();
        }

        public async Task DeleteLineAsync(int lineId, bool trackChanges)
        {
            var line = await GetLineAndCheckIfItExists(lineId, trackChanges);
            _repository.Line.DeleteLine(line);
            await _repository.SaveAsync();
        }

        private async Task<Line> GetLineAndCheckIfItExists(int id, bool trackChanges)
        {
            var line = await _repository.Line.GetLineByIdAsync(id, trackChanges);
            if (line is null)
                throw new LineNotFoundException(id);
            return line;
        }
    }
}