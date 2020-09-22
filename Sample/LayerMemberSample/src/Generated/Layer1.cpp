#include "./Generated/API.h"
#include "./Generated/Layer_Private.h"
#include "./Generated/LayerdObject_Private.h"
#include "RTCOP/Framework.h"
#include "RTCOP/Core/RTCOPManager.h"
#include "./Generated/DependentCode.h"
#include <iostream>
#include <cstring>

#include "stdio.h"

#include "./Generated/BaseLayer.h"
#include "./Generated/Layer1.h"

namespace RTCOP {
namespace Generated {

Layer1* Layer1::GetInstance()
{
	return (Layer1*)RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer((int)LayerID::Layer1);
}

Layer1::Layer1(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods)
	:RTCOP::Core::Layer(id, name, numOfBaseClasses, numOfBaseMethods)
{
	int size0 = sizeof(volatile void*) * numOfBaseMethods[0];
	volatile void** virtualFunctionTable0	 = DependentCode::baselayer::Sample::GetVirtualFunctionTable(this);
	std::memcpy(_Private->_VirtualFunctionTables[0], virtualFunctionTable0, size0);
	_Private->_VirtualFunctionTables[0][1] = 0;
}

Layer1::~Layer1()
{
}

void* Layer1::InitializeLayerdObject(void* obj, int classID)
{
	int layerID = _Private->_ID;
	if (classID == 0)
	{
		::Layer1::Sample* layerdObject = reinterpret_cast<::Layer1::Sample*>(obj);
		layerdObject->_Private->_PartialClassMembers[layerID] = new ::Layer1::Sample::PartialClassMembers();
		layerdObject->_Private->_PartialClassMembers[layerID]->_Layer = this;
		volatile void* vfp = DependentCode::GetLayerdObjectFinalizer(layerdObject);
		layerdObject->_Private->_PartialClassMembers[layerID]->_Finalizer = vfp;
		layerdObject->_RTCOP_InitializePartialClass();
	}
	return obj;
}

void Layer1::_RTCOP_OnActivating()
{
}

void Layer1::_RTCOP_OnActivated()
{
}

void Layer1::_RTCOP_OnDeactivating()
{
}

void Layer1::_RTCOP_OnDeactivated()
{
}

} // namespace Generated {}
} // namespace RTCOP {}

namespace Layer1
{
int Sample :: _LayerMember2 ;
void Sample::InitializeLayerMembers ()
{
	Sample::PartialClassMembers* layer_members = (Sample::PartialClassMembers*)_Private->_PartialClassMembers[1];
	Layer1 :: Sample :: _LayerMember2 = 30 ;
	layer_members -> _LayerMember1 = _BaseMember + Layer1 :: Sample :: _LayerMember2 ;

}

void Sample::Print ()
{
	Sample::PartialClassMembers* layer_members = (Sample::PartialClassMembers*)_Private->_PartialClassMembers[1];
	volatile void* _RTCOP_proceedaddr = RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer(1)->GetVirtualFunctionTableForProceeding(0)[0];
	auto proceed = [this, _RTCOP_proceedaddr]() { RTCOP::Generated::DependentCode::baselayer::Sample::ExecuteProceed_Print(this, _RTCOP_proceedaddr); };
	printf ( "_BaseMember:%d _LayerMember:%d\n" , _BaseMember , layer_members -> _LayerMember1 ) ;

}


}


namespace Layer1{

Sample::Sample(int value)
	: RTCOP::Core::LayerdObject<baselayer::Sample>(value)
{
}

void Sample::_RTCOP_InitializePartialClass()
{
	InitializeLayerMembers();
}

void Sample::_RTCOP_FinalizePartialClass()
{
}


}

