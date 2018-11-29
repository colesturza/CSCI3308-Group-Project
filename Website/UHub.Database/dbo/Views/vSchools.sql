
create view [dbo].[vSchools]
as

	with set1 as
	(
		select 
			EntID,
			--
			case when epx.PropID = 2
			then PropValue
			else NULL
			end [Name],
			--
			case when epx.PropID = 15
			then PropValue
			else NULL
			end [State],
			--
			case when epx.PropID = 16
			then PropValue
			else NULL
			end [City],
			--
			case when epx.PropID = 17
			then PropValue
			else NULL
			end [DomainValidator],
			--
			case when epx.PropID = 24
			then PropValue
			else NULL
			end [Description]

		from
			EntPropertyXRef epx
		where
			epx.EntTypeID = 2
	),
	set2 as
	(
		select
			EntID,
			min([Name])				[Name],
			min([State])			[State],
			min(City)				City,
			min(DomainValidator)	DomainValidator,
			min([Description])		[Description]
		from set1
		group by
			EntID
	)

	select
		ent.ID,
		ent.EntTypeID,
		ent.IsEnabled,
		ent.IsReadonly,
		s2.[Name],
		s2.[State],
		s2.City,
		s2.DomainValidator,
		s2.[Description],
		ent.IsDeleted,
		ent.CreatedBy,
		ent.CreatedDate,
		ent.ModifiedBy,
		ent.ModifiedDate,
		ent.DeletedBy,
		ent.DeletedDate
	from Entities ent

	inner join set2 s2
	on
		ent.ID = s2.EntID

	

	where
		ent.IsDeleted = 0









