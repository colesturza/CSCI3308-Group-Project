
create view vComments

as
	with set1 as
	(
		select 
			EntID,
			--
			case when epx.PropID = 12
			then PropValue
			else NULL
			end [Content],
			--
			case when epx.PropID = 13
			then PropValue
			else NULL
			end [IsModified],
			--
			case when epx.PropID = 14
			then PropValue
			else NULL
			end [ViewCount]

		from
			EntPropertyXRef epx
		where
			epx.EntTypeID = 7
	),
	set2 as
	(
		select
			EntID,
			min(content)					Content,
			cast(min(IsModified) as bit)	IsModified,
			cast(min(ViewCount) as bigint)	ViewCount
		from set1
		group by
			EntID
	)

	select
		ent.ID,
		ent.EntTypeID,
		ent.IsEnabled,
		ent.IsReadonly,
		s2.Content					as [Content],
		s2.IsModified				as [IsModified],
		s2.ViewCount				as [ViewCount],
		xref_Parent.ParentEntID		as [ParentID],
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

	inner join dbo.EntChildXRef xref_Parent
	on
		xref_Parent.ChildEntID = ent.ID

	where
		ent.IsDeleted = 0







