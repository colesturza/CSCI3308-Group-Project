




CREATE view [dbo].[vPosts]
as

	--select * from vPosts
	with set1 as
	(
		select 
			EntID,
			case when epx.PropID = 2
			then PropValue
			else NULL
			end [Name],
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
			end [ViewCount],
			--
			case when epx.PropID = 33
			then PropValue
			else NULL
			end [IsLocked],
			--
			case when epx.PropID = 34
			then PropValue
			else NULL
			end [CanComment],
			--
			case when epx.PropID = 35
			then PropValue
			else NULL
			end [IsPublic]

		from
			dbo.EntPropertyXRef epx
		where
			epx.EntTypeID = 6
	),
	set2 as
	(
		select
			EntID,
			min([name])						[Name],
			min(content)					Content,
			cast(min(IsModified) as bit)	IsModified,
			cast(min(ViewCount) as bigint)	ViewCount,
			cast(min(IsLocked) as bit)		IsLocked,
			cast(min(CanComment) as bit)	CanComment,
			cast(min(IsPublic) as bit)		IsPublic
		from set1
		group by
			EntID
	)

	select
		ent.ID,
		ent.EntTypeID,
		ent.IsEnabled,
		ent.IsReadonly,
		s2.[Name]					as [Name],
		s2.Content					as [Content],
		s2.IsModified				as [IsModified],
		s2.ViewCount				as [ViewCount],
		s2.IsLocked					as [IsLocked],
		s2.CanComment				as [CanComment],
		s2.IsPublic					as [IsPublic],
		xref_Parent.ParentEntID		as [ParentID],
		ent.IsDeleted,
		ent.CreatedBy,
		ent.CreatedDate,
		ent.ModifiedBy,
		ent.ModifiedDate,
		ent.DeletedBy,
		ent.DeletedDate
	from dbo.Entities ent

	inner join set2 s2
	on
		ent.ID = s2.EntID

	inner join dbo.EntChildXRef xref_Parent
	on
		xref_Parent.ChildEntID = ent.ID

	where
		ent.IsDeleted = 0









