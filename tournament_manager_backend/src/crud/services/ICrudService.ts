export interface ICrudService<TEntity, TCreateDto, TUpdateDto> {
    create(dto: TCreateDto): Promise<TEntity>;
    findAll(): Promise<TEntity[]>;
    findOne(id: number): Promise<TEntity | null>;
    update(id: number, dto: TUpdateDto): Promise<TEntity>;
    remove(id: number): Promise<void>;
}
