create view vPosts_ClusteredCounts
as

with clubCounterPublic as
(
	select
		ecx.ParentEntID		as SchoolClubID,
		count(*)			as PostCount
	from dbo.EntChildXRef ecx		--POST --> CLUB
	inner join EntPropertyXRef epx
	on
		ecx.ChildEntID = epx.EntID
		and epx.PropID = 35			--IS PUBLIC [35]
		and epx.PropValue = '1'
	where
		ecx.ChildEntType = 6		--POST TYPE [6]
		AND ecx.ParentEntType = 4	--CLUB TYPE [4]
		AND ecx.IsDeleted = 0
	group by 
		ecx.ParentEntID

), clubCounterPrivate as 
(

	select
		ecx.ParentEntID		as SchoolClubID,
		count(*)			as PostCount
	from dbo.EntChildXRef ecx		--POST --> CLUB
	inner join EntPropertyXRef epx
	on
		ecx.ChildEntID = epx.EntID
		and epx.PropID = 35			--IS PUBLIC [35]
		and epx.PropValue = '0'
	where
		ecx.ChildEntType = 6		--POST TYPE [6]
		AND ecx.ParentEntType = 4	--CLUB TYPE [4]
		AND ecx.IsDeleted = 0
	group by 
		ecx.ParentEntID


)
select
	ecx.ParentEntID					as SchoolID,
	ccPub.SchoolClubID				as SchoolClubID,
	ccPub.PostCount					as PublicPostCount,
	COALESCE(ccPriv.PostCount, 0)	as PrivatePostCount
from clubCounterPublic ccPub
left join clubCounterPrivate ccPriv
on
	ccPub.SchoolClubID = ccPriv.SchoolClubID
inner join EntChildXRef ecx		-- CLUB --> SCHOOL
on
	ccPub.SchoolClubID = ecx.ChildEntID
	and ecx.ParentEntType = 2	--SCHOOL TYPE [2]
	and ecx.IsDeleted = 0


UNION ALL


select
	ParentEntID		as SchoolID,
	null			as SchoolClubID,
	count(*)		as PublicPostCount,
	0				as PrivatePostCount
from dbo.EntChildXRef ecx
where
	ChildEntType = 6		--POST TYPE [6]
	AND ParentEntType = 2	--SCHOOL TYPE [1]
	AND IsDeleted = 0
group by 
	ParentEntID