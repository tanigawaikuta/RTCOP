#include "./Generated/API.h"
#include "./Generated/Layer_Private.h"
#include "./Generated/LayerdObject_Private.h"
#include "./Generated/DependentCode.h"
#include "RTCOP/Framework.h"
#include "RTCOP/Core/RTCOPManager.h"
#include "RTCOP/Core/PartialClassMembers.h"
#include <cstring>

#include "./Generated/API.h"
#include "stdio.h"

#include "./Generated/BaseLayer.h"

namespace RTCOP {
namespace Generated {

BaseLayer* BaseLayer::GetInstance()
{
	return (BaseLayer*)RTCOP::Framework::Instance->GetRTCOPManager()->GetLayer((int)LayerID::baselayer);
}

BaseLayer::BaseLayer(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods)
	:Core::Layer(id, name, numOfBaseClasses, numOfBaseMethods)
{
	int size0 = sizeof(volatile void*) * numOfBaseMethods[0];
	volatile void** virtualFunctionTable0 = DependentCode::baselayer::SuperA::GetVirtualFunctionTable(this);
	std::memcpy(_Private->_VirtualFunctionTables[0], virtualFunctionTable0, size0);
	int size1 = sizeof(volatile void*) * numOfBaseMethods[1];
	volatile void** virtualFunctionTable1 = DependentCode::baselayer::SubA::GetVirtualFunctionTable(this);
	std::memcpy(_Private->_VirtualFunctionTables[1], virtualFunctionTable1, size1);
}

BaseLayer::~BaseLayer()
{
}

void* BaseLayer::InitializeLayerdObject(void* obj, int classID)
{
	return obj;
}

} // namespace Generated {}
} // namespace RTCOP {}

using namespace RTCOP ;
namespace baselayer
{
SuperA :: SuperA ( ) 
{

}

SuperA :: ~ SuperA ( ) 
{

}

void SuperA::m1 ()
{
	printf ( "baselayer::SuperA::m1()\n" ) ;

}

SubA :: SubA ( ) 
{

}

SubA :: ~ SubA ( ) 
{

}

void SubA::m1 ()
{
	volatile void** _RTCOP_vft = RTCOP::Framework::Instance->GetRTCOPManager()->GetVirtualFunctionTable(0);
	auto supercall = [this, _RTCOP_vft]() { RTCOP::Generated::DependentCode::baselayer::SubA::ExecuteProceed_m1(this, _RTCOP_vft[1]); };
	printf ( "baselayer::SubA::m1()\n" ) ;
	supercall ( ) ;

}


}


